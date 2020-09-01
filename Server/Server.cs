using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using MessageCustomHandler;

namespace Server
{
    public sealed class ServerClient : IDisposable
    {
        public event ExceptionThrownEventHandler ExceptionThrown;
        public delegate void ExceptionThrownEventHandler(ServerClient Sender, Exception Ex);
        public event WriteProgressChangedEventHandler WriteProgressChanged;
        public delegate void WriteProgressChangedEventHandler(ServerClient Sender, double Progress, int BytesWritten, int BytesToWrite);
        public event ReadProgressChangedEventHandler ReadProgressChanged;
        public delegate void ReadProgressChangedEventHandler(ServerClient Sender, double Progress, int BytesRead, int BytesToRead);
        public event WritePacketEventHandler WritePacket;
        public delegate void WritePacketEventHandler(ServerClient Sender, int Size);
        public event ReadPacketEventHandler ReadPacket;
        public delegate void ReadPacketEventHandler(ServerClient Sender, byte[] Data);
        public event StateChangedEventHandler StateChanged;
        public delegate void StateChangedEventHandler(ServerClient Sender, bool Connected);
        private SocketAsyncEventArgs[] Items = new SocketAsyncEventArgs[2];
        private bool[] Processing = new bool[2];
        private ushort CBufferSize = 8192;
        private int CMaxPacketSize = 10485760;
        private object CUserState;
        private IPEndPoint CEndPoint;
        private Socket Handle;
        private int SendIndex;
        private byte[] SendBuffer;
        private int ReadIndex;
        private byte[] ReadBuffer;
        private Queue<byte[]> SendQueue;
        private bool DisposedValue;

        private void OnExceptionThrown(Exception Ex)
        {
            ExceptionThrown?.Invoke(this, Ex);
        }

        private void OnStateChanged(bool Connected)
        {
            StateChanged?.Invoke(this, Connected);
        }

        private void OnReadPacket(byte[] Data)
        {
            ReadPacket?.Invoke(this, Data);
        }

        private void OnReadProgressChanged(double Progress, int BytesRead, int BytesToRead)
        {
            ReadProgressChanged?.Invoke(this, Progress, BytesRead, BytesToRead);
        }

        private void OnWritePacket(int Size)
        {
            WritePacket?.Invoke(this, Size);
        }

        private void OnWriteProgressChanged(double progress, int bytesWritten, int bytesToWrite)
        {
            WriteProgressChanged?.Invoke(this, progress, bytesWritten, bytesToWrite);
        }

        public ushort BufferSize
        {
            get
            {
                return CBufferSize;
            }
        }

        public int MaxPacketSize
        {
            get
            {
                return CMaxPacketSize;
            }
            set
            {
                if (value < 1)
                    throw new Exception("Value must be greater than 0.");
                else
                    CMaxPacketSize = value;
            }
        }

        public User UserState
        {
            get
            {
                return (User)CUserState;
            }
            set
            {
                CUserState = value;
            }
        }

        public IPEndPoint EndPoint
        {
            get
            {
                if (CEndPoint != null)
                    return CEndPoint;
                else
                    return new IPEndPoint(IPAddress.None, 0);
            }
        }

        public bool Connected { get; private set; }

        public ServerClient(Socket sock, ushort bufferSize, int maxPacketSize)
        {
            try
            {
                Initialize();
                Items[0].SetBuffer(new byte[bufferSize - 1 + 1], 0, bufferSize);
                Handle = sock;
                CBufferSize = bufferSize;
                CMaxPacketSize = maxPacketSize;
                CEndPoint = (IPEndPoint)Handle.RemoteEndPoint;
                Connected = true;
                if (!Handle.ReceiveAsync(Items[0]))
                    Process(null, Items[0]);
            }
            catch (Exception ex)
            {
                OnExceptionThrown(ex);
                Disconnect();
            }
        }

        private void Initialize()
        {
            Processing = new bool[2];
            SendIndex = 0;
            ReadIndex = 0;
            SendBuffer = new byte[0] { };
            ReadBuffer = new byte[0] { };
            SendQueue = new Queue<byte[]>();
            Items = new SocketAsyncEventArgs[2];
            Items[0] = new SocketAsyncEventArgs();
            Items[1] = new SocketAsyncEventArgs();
            Items[0].Completed += Process;
            Items[1].Completed += Process;
        }

        private void Process(object s, SocketAsyncEventArgs e)
        {
            try
            {
                if (e.SocketError == SocketError.Success)
                {
                    switch (e.LastOperation)
                    {
                        case SocketAsyncOperation.Receive:
                            {
                                if (!Connected)
                                    return;
                                if (e.BytesTransferred != 0)
                                {
                                    HandleRead(e.Buffer, 0, e.BytesTransferred);
                                    if (!Handle.ReceiveAsync(e))
                                        Process(null, e);
                                }
                                else
                                    Disconnect();
                                break;
                            }

                        case SocketAsyncOperation.Send:
                            {
                                if (!Connected)
                                    return;
                                bool EOS = false;
                                SendIndex += e.BytesTransferred;
                                OnWriteProgressChanged((SendIndex / (double)SendBuffer.Length) * 100, SendIndex, SendBuffer.Length);
                                if ((SendIndex >= SendBuffer.Length))
                                {
                                    EOS = true;
                                    OnWritePacket(SendBuffer.Length - 4);
                                }
                                if (SendQueue.Count == 0 && EOS)
                                    Processing[1] = false;
                                else
                                    HandleSendQueue();
                                break;
                            }
                    }
                }
                else
                {
                    OnExceptionThrown(new SocketException((int)e.SocketError));
                    Disconnect();
                }
            }
            catch (Exception ex)
            {
                OnExceptionThrown(ex);
                Disconnect();
            }
        }

        public void Disconnect()
        {
            try
            {
                if (Processing[0])
                    return;
                else
                    Processing[0] = true;
                bool Raise = Connected;
                Connected = false;
                if (Handle != null)
                    Handle.Close();
                if (SendQueue != null)
                    SendQueue.Clear();
                SendBuffer = new byte[0] { };
                ReadBuffer = new byte[0] { };
                if (Raise)
                    OnStateChanged(false);
                if (Items != null)
                {
                    Items[0].Dispose();
                    Items[1].Dispose();
                }
                CUserState = null;
                CEndPoint = null;
            }
            catch (Exception ex)
            {
                CMBox.Show("Error", "Error disconnecting, " + ex.Message, Style.Error, Buttons.OK, ex.ToString());
            }
        }

        public void Send(byte[] data)
        {
            if (!Connected)
                return;
            SendQueue.Enqueue(data);
            if (!Processing[1])
            {
                Processing[1] = true;
                HandleSendQueue();
            }
        }

        private void HandleSendQueue()
        {
            try
            {
                if (SendIndex >= SendBuffer.Length)
                {
                    SendIndex = 0;
                    SendBuffer = Header(SendQueue.Dequeue());
                }
                int Write = Math.Min(SendBuffer.Length - SendIndex, CBufferSize);
                Items[1].SetBuffer(SendBuffer, SendIndex, Write);
                if (!Handle.SendAsync(Items[1]))
                    Process(null, Items[1]);
            }
            catch (Exception ex)
            {
                OnExceptionThrown(ex);
                Disconnect();
            }
        }

        private static byte[] Header(byte[] data)
        {
            byte[] T = new byte[data.Length + 3 + 1];
            Buffer.BlockCopy(BitConverter.GetBytes(data.Length), 0, T, 0, 4);
            Buffer.BlockCopy(data, 0, T, 4, data.Length);
            return T;
        }

        private void HandleRead(byte[] data, int index, int length)
        {
            if (ReadIndex >= ReadBuffer.Length)
            {
                ReadIndex = 0;
                if (data.Length < 4)
                {
                    OnExceptionThrown(new Exception("Missing or corrupt packet header."));
                    Disconnect();
                    return;
                }
                int PacketSize = BitConverter.ToInt32(data, index);
                if (PacketSize > CMaxPacketSize)
                {
                    OnExceptionThrown(new Exception("Packet size exceeds MaxPacketSize."));
                    Disconnect();
                    return;
                }
                Array.Resize(ref ReadBuffer, PacketSize);
                index += 4;
            }
            int Read = Math.Min(ReadBuffer.Length - ReadIndex, length - index);
            Buffer.BlockCopy(data, index, ReadBuffer, ReadIndex, Read);
            ReadIndex += Read;
            OnReadProgressChanged((ReadIndex / (double)ReadBuffer.Length) * 100, ReadIndex, ReadBuffer.Length);
            if (ReadIndex >= ReadBuffer.Length)
                OnReadPacket(ReadBuffer);
            if (Read < (length - index))
                HandleRead(data, index + Read, length);
        }

        private void Dispose(bool disposing)
        {
            if (!DisposedValue && disposing)
                Disconnect();
            DisposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    sealed class ServerListener : IDisposable
    {
        public event ClientWriteProgressChangedEventHandler ClientWriteProgressChanged;
        public delegate void ClientWriteProgressChangedEventHandler(ServerListener Sender, ServerClient Client, double Progress, int BytesWritten, int BytesToWrite);
        public event StateChangedEventHandler StateChanged;
        public delegate void StateChangedEventHandler(ServerListener Sender, bool Listening);
        public event ExceptionThrownEventHandler ExceptionThrown;
        public delegate void ExceptionThrownEventHandler(ServerListener Sender, Exception Ex);
        public event ClientExceptionThrownEventHandler ClientExceptionThrown;
        public delegate void ClientExceptionThrownEventHandler(ServerListener sender, ServerClient Client, Exception Ex);
        public event ClientStateChangedEventHandler ClientStateChanged;
        public delegate void ClientStateChangedEventHandler(ServerListener Sender, ServerClient Client, bool Connected);
        public event ClientReadPacketEventHandler ClientReadPacket;
        public delegate void ClientReadPacketEventHandler(ServerListener Sender, ServerClient Client, byte[] Data);
        public event ClientReadProgressChangedEventHandler ClientReadProgressChanged;
        public delegate void ClientReadProgressChangedEventHandler(ServerListener Sender, ServerClient Client, double Progress, int BytesRead, int BytesToRead);
        public event ClientWritePacketEventHandler ClientWritePacket;
        public delegate void ClientWritePacketEventHandler(ServerListener Sender, ServerClient Client, int Size);
        private ushort CBufferSize = 8192;
        private int CMaxPacketSize = 10485760;
        private bool CKeepAlive = true;
        private ushort CMaxConnections = 20;
        private bool CListening;
        private List<ServerClient> CClients;
        private Socket Handle;
        private bool Processing;
        private SocketAsyncEventArgs Item;
        private bool DisposedValue;

        private void OnStateChanged(bool listening)
        {
            StateChanged?.Invoke(this, listening);
        }

        private void OnExceptionThrown(Exception ex)
        {
            ExceptionThrown?.Invoke(this, ex);
        }

        private void OnClientExceptionThrown(ServerClient client, Exception ex)
        {
            ClientExceptionThrown?.Invoke(this, client, ex);
        }

        private void OnClientStateChanged(ServerClient client, bool connected)
        {
            ClientStateChanged?.Invoke(this, client, connected);
        }

        private void OnClientReadPacket(ServerClient client, byte[] data)
        {
            ClientReadPacket?.Invoke(this, client, data);
        }

        private void OnClientReadProgressChanged(ServerClient client, double progress, int bytesRead, int bytesToRead)
        {
            ClientReadProgressChanged?.Invoke(this, client, progress, bytesRead, bytesToRead);
        }

        private void OnClientWritePacket(ServerClient client, int size)
        {
            ClientWritePacket?.Invoke(this, client, size);
        }

        private void OnClientWriteProgressChanged(ServerClient client, double progress, int bytesWritten, int bytesToWrite)
        {
            ClientWriteProgressChanged?.Invoke(this, client, progress, bytesWritten, bytesToWrite);
        }

        public ushort BufferSize
        {
            get
            {
                return CBufferSize;
            }
            set
            {
                if (value < 1)
                    throw new Exception("Value must be greater than 0.");
                else
                    CBufferSize = value;
            }
        }

        public int MaxPacketSize
        {
            get
            {
                return CMaxPacketSize;
            }
            set
            {
                if (value < 1)
                    throw new Exception("Value must be greater than 0.");
                else
                    CMaxPacketSize = value;
            }
        }

        public bool KeepAlive
        {
            get
            {
                return CKeepAlive;
            }
            set
            {
                if (CListening)
                    throw new Exception("Unable to change this option while listening.");
                else
                    CKeepAlive = value;
            }
        }

        public ushort MaxConnections
        {
            get
            {
                return CMaxConnections;
            }
            set
            {
                CMaxConnections = value;
            }
        }

        public bool Listening
        {
            get
            {
                return CListening;
            }
        }

        public ServerClient[] Clients
        {
            get
            {
                if (CListening)
                    return CClients.ToArray();
                else
                    return new ServerClient[] { };
            }
        }

        public void Listen(ushort port)
        {
            try
            {
                if (!CListening)
                {
                    CClients = new List<ServerClient>();
                    Item = new SocketAsyncEventArgs();
                    Item.Completed += Process;
                    Item.AcceptSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                    {
                        NoDelay = true
                    };
                    if (CKeepAlive)
                        Item.AcceptSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                    Handle = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    Handle.Bind(new IPEndPoint(IPAddress.Any, port));
                    Handle.Listen(10);
                    Processing = false;
                    CListening = true;
                    OnStateChanged(true);
                    if (!Handle.AcceptAsync(Item))
                        Process(null, Item);
                }
            }
            catch (Exception ex)
            {
                OnExceptionThrown(ex);
                Disconnect();
            }
        }

        private void Process(object s, SocketAsyncEventArgs e)
        {
            try
            {
                if (e.SocketError == SocketError.Success)
                {
                    ServerClient T = new ServerClient(e.AcceptSocket, CBufferSize, CMaxPacketSize);
                    lock (CClients)
                    {
                        if (CClients.Count < CMaxConnections)
                        {
                            CClients.Add(T);
                            T.StateChanged += HandleStateChanged;
                            T.ExceptionThrown += OnClientExceptionThrown;
                            T.ReadPacket += OnClientReadPacket;
                            T.ReadProgressChanged += OnClientReadProgressChanged;
                            T.WritePacket += OnClientWritePacket;
                            T.WriteProgressChanged += OnClientWriteProgressChanged;
                            OnClientStateChanged(T, true);
                        }
                        else
                            T.Disconnect();
                    }
                    e.AcceptSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                    {
                        NoDelay = true
                    };
                    if (CKeepAlive)
                        e.AcceptSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                    if (!Handle.AcceptAsync(e))
                        Process(null, e);
                }
                else
                {
                    OnExceptionThrown(new SocketException((int)e.SocketError));
                    Disconnect();
                }
            }
            catch (Exception ex)
            {
                OnExceptionThrown(ex);
                Disconnect();
            }
        }

        public void Disconnect()
        {
            try
            {
                if (Processing)
                    return;
                else
                    Processing = true;
                if (Handle != null)
                    Handle.Close();
                if (CClients != null)
                {
                    lock (CClients)
                    {
                        while (CClients.Count > 0)
                        {
                            CClients[0].Disconnect();
                            if (CClients.Count > 0)
                                CClients.RemoveAt(0);
                        }
                    }
                }
                if (Item != null)
                    Item.Dispose();
                CListening = false;
                OnStateChanged(false);
            }
            catch (Exception ex)
            {
                CMBox.Show("Error", "Error disconnecting, " + ex.Message, Style.Error, Buttons.OK, ex.ToString());
            }
        }

        private void HandleStateChanged(ServerClient client, bool connected)
        {
            lock (CClients)
            {
                CClients.Remove(client);
                OnClientStateChanged(client, false);
            }
        }

        private void Dispose(bool disposing)
        {
            if (!DisposedValue && disposing)
                Disconnect();
            DisposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
