using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using CoreAudio.Enumerations;
using CoreAudio.Interfaces;

namespace Server
{
    public static class VolumeMixer
    {

        public static IEnumerable<ApplicationVolumeInformation> EnumerateApplications()
        {
            // get the speakers (1st render + multimedia) device
            IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)(new MMDeviceEnumerator());
            deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out IMMDevice speakers);

            // activate the session manager. we need the enumerator
            Guid IID_IAudioSessionManager2 = typeof(IAudioSessionManager2).GUID;
            speakers.Activate(IID_IAudioSessionManager2, 0, IntPtr.Zero, out object o);
            IAudioSessionManager2 mgr = (IAudioSessionManager2)o;

            // enumerate sessions for on this device
            mgr.GetSessionEnumerator(out IAudioSessionEnumerator sessionEnumerator);
            sessionEnumerator.GetCount(out int count);

            for (int i = 0; i < count; i++)
            {
                sessionEnumerator.GetSession(i, out IAudioSessionControl ctl);

                uint GetProcessID = 0;
                string GetName = "";
                float GetVolume = 0;
                string GetIconPath = "";

                IAudioSessionControl getsession = ctl;
                if (ctl is IAudioSessionControl2 ctl2)
                {
                    ctl2.GetProcessId(out GetProcessID);
                    ctl2.GetDisplayName(out GetName);

                    ctl2.GetIconPath(out string sIconPath);

                    GetIconPath = sIconPath;

                    ISimpleAudioVolume volcast = (ctl2 as ISimpleAudioVolume);
                    volcast.GetMasterVolume(out float grabvolume);
                    GetVolume = grabvolume;
                    try
                    {
                        Process grabProcess = Process.GetProcessById((int)GetProcessID);
                        if (string.IsNullOrEmpty(GetName))
                        {
                            GetName = grabProcess.ProcessName;
                        }
                    }
                    catch
                    {
                        GetName = "Name Not Available";
                    }

                }
                ApplicationVolumeInformation avi = new ApplicationVolumeInformation(getsession, GetProcessID, GetVolume, GetName, GetIconPath);

                yield return avi;
                Marshal.ReleaseComObject(ctl);
            }
            Marshal.ReleaseComObject(sessionEnumerator);
            Marshal.ReleaseComObject(mgr);
            Marshal.ReleaseComObject(speakers);
            Marshal.ReleaseComObject(deviceEnumerator);
        }
        public static float GetMasterVolume()
        {
            // retrieve audio device...
            IMMDeviceEnumerator useenumerator = (IMMDeviceEnumerator)(new MMDeviceEnumerator());

            //retrieve the actual endpoint

            useenumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out IMMDevice speakers);

            //retrieve the actual interface instance to retrieve the volume information from.
            speakers.Activate(typeof(IAudioEndpointVolume).GUID, 0, IntPtr.Zero, out object o);
            IAudioEndpointVolume aepv = (IAudioEndpointVolume)o;
            aepv.GetMasterVolumeLevelScalar(out float result);
            Marshal.ReleaseComObject(aepv);
            Marshal.ReleaseComObject(speakers);
            Marshal.ReleaseComObject(useenumerator);
            return result;
        }

        public static float SetMasterVolume(float newValue)
        {
            // retrieve audio device...

            IMMDeviceEnumerator useenumerator = (IMMDeviceEnumerator)(new MMDeviceEnumerator());
            const int eRender = 0;
            //const int eMultimedia = 1;
            //retrieve the actual endpoint
            useenumerator.GetDefaultAudioEndpoint(eRender, ERole.eMultimedia, out IMMDevice speakers);

            //retrieve the actual interface instance to retrieve the volume information from.
            speakers.Activate(typeof(IAudioEndpointVolume).GUID, 0, IntPtr.Zero, out object o);
            IAudioEndpointVolume aepv = (IAudioEndpointVolume)o;
            _ = aepv.GetMasterVolumeLevelScalar(out float result);
            aepv.SetMasterVolumeLevelScalar(newValue, new Guid());
            Marshal.ReleaseComObject(aepv);
            Marshal.ReleaseComObject(speakers);
            Marshal.ReleaseComObject(useenumerator);
            return result;
        }

        public static float? GetApplicationVolume(string name)
        {
            ISimpleAudioVolume volume = GetVolumeObject(name);
            if (volume == null)
                return null;

            volume.GetMasterVolume(out float level);
            return level * 100;
        }

        public static float? GetApplicationVolume(int procID)
        {
            ISimpleAudioVolume volume = GetVolumeObject(procID);
            if (volume == null)
                return null;

            volume.GetMasterVolume(out float level);
            return level * 100;
        }

        public static bool? GetApplicationMute(string name)
        {
            ISimpleAudioVolume volume = GetVolumeObject(name);
            if (volume == null)
                return null;

            volume.GetMute(out bool mute);
            return mute;
        }

        public static bool? GetApplicationMute(int procID)
        {
            ISimpleAudioVolume volume = GetVolumeObject(procID);
            if (volume == null)
                return null;

            volume.GetMute(out bool mute);
            return mute;
        }

        public static ApplicationVolumeInformation GetApplicationVolumeInfo(string sName)
        {
            foreach (var iterateNode in EnumerateApplications())
            {
                if (iterateNode.Name.Equals(sName, StringComparison.OrdinalIgnoreCase))
                {
                    return iterateNode;
                }
            }
            return null;
        }

        public static ApplicationVolumeInformation GetApplicationVolumeInfo(int ProcID)
        {
            foreach (var iterateNode in EnumerateApplications())
            {
                if (iterateNode.ProcessID.Equals(ProcID))
                {
                    return iterateNode;
                }
            }
            return null;
        }

        public static void SetApplicationVolume(string name, float level)
        {
            ISimpleAudioVolume volume = GetVolumeObject(name);
            if (level < 1) level *= 100;
            if (volume == null)
                return;

            Guid guid = Guid.Empty;
            volume.SetMasterVolume(level / 100, guid);
        }

        public static void SetApplicationVolume(int procID, float level)
        {
            ISimpleAudioVolume volume = GetVolumeObject(procID);
            if (level < 1) level *= 100;
            if (volume == null)
                return;

            Guid guid = Guid.Empty;
            volume.SetMasterVolume(level / 100, guid);
        }

        public static void SetApplicationMute(string name, bool mute)
        {
            ISimpleAudioVolume volume = GetVolumeObject(name);
            if (volume == null)
                return;

            Guid guid = Guid.Empty;
            volume.SetMute(mute, guid);
        }

        public static void SetApplicationMute(int procID, bool mute)
        {
            ISimpleAudioVolume volume = GetVolumeObject(procID);
            if (volume == null)
                return;

            Guid guid = Guid.Empty;
            volume.SetMute(mute, guid);
        }

        private static ISimpleAudioVolume GetVolumeObject(string name)
        {
            // get the speakers (1st render + multimedia) device
            IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)(new MMDeviceEnumerator());
            deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out IMMDevice speakers);

            // activate the session manager. we need the enumerator
            Guid IID_IAudioSessionManager2 = typeof(IAudioSessionManager2).GUID;
            speakers.Activate(IID_IAudioSessionManager2, 0, IntPtr.Zero, out object o);
            IAudioSessionManager2 mgr = (IAudioSessionManager2)o;

            // enumerate sessions for on this device
            mgr.GetSessionEnumerator(out IAudioSessionEnumerator sessionEnumerator);
            sessionEnumerator.GetCount(out int count);

            // search for an audio session with the required name
            // NOTE: we could also use the process id instead of the app name (with IAudioSessionControl2)
            ISimpleAudioVolume volumeControl = null;
            for (int i = 0; i < count; i++)
            {
                sessionEnumerator.GetSession(i, out IAudioSessionControl ctl);
                ctl.GetDisplayName(out string dn);
                if (string.Compare(name, dn, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    volumeControl = ctl as ISimpleAudioVolume;
                    break;
                }
                Marshal.ReleaseComObject(ctl);
            }

            Marshal.ReleaseComObject(sessionEnumerator);
            Marshal.ReleaseComObject(mgr);
            Marshal.ReleaseComObject(speakers);
            Marshal.ReleaseComObject(deviceEnumerator);

            return volumeControl;
        }

        private static ISimpleAudioVolume GetVolumeObject(int procID)
        {
            // get the speakers (1st render + multimedia) device
            IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)(new MMDeviceEnumerator());
            deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out IMMDevice speakers);

            // activate the session manager. we need the enumerator
            Guid IID_IAudioSessionManager2 = typeof(IAudioSessionManager2).GUID;
            speakers.Activate(IID_IAudioSessionManager2, 0, IntPtr.Zero, out object o);
            IAudioSessionManager2 mgr = (IAudioSessionManager2)o;

            // enumerate sessions for on this device
            mgr.GetSessionEnumerator(out IAudioSessionEnumerator sessionEnumerator);
            sessionEnumerator.GetCount(out int count);

            // search for an audio session with the required process ID
            ISimpleAudioVolume volumeControl = null;
            for (int i = 0; i < count; i++)
            {
                sessionEnumerator.GetSession(i, out IAudioSessionControl ctl);

                if (ctl is IAudioSessionControl2 ctl2)
                {
                    ctl2.GetProcessId(out uint cProcId);

                    if (cProcId == procID)
                    {
                        volumeControl = ctl as ISimpleAudioVolume;
                        break;
                    }
                }

                Marshal.ReleaseComObject(ctl);
            }

            Marshal.ReleaseComObject(sessionEnumerator);
            Marshal.ReleaseComObject(mgr);
            Marshal.ReleaseComObject(speakers);
            Marshal.ReleaseComObject(deviceEnumerator);

            return volumeControl;
        }

        [ComImport]
        [Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
        private class MMDeviceEnumerator
        {
        }

        public class ApplicationVolumeInformation
        {
            private IAudioSessionControl AudioSession { get; set; }
            public uint ProcessID { get; set; }
            private float _Volume = 0;

            public float Volume
            {
                get
                {
                    return _Volume;
                }
                set
                {
                    _Volume = value;
                    if (AudioSession != null)
                    {
                        ISimpleAudioVolume vol = AudioSession as ISimpleAudioVolume;
                        vol?.SetMasterVolume(_Volume, Guid.Empty);
                    }
                }
            }

            public string Name { get; private set; }

            public string IconPath { get; private set; }

            public ApplicationVolumeInformation(IAudioSessionControl Session, uint pProcessID, float pVolume, string pName, string pIconPath)
            {
                AudioSession = Session;
                ProcessID = pProcessID;
                _Volume = pVolume;
                Name = pName;
                IconPath = pIconPath;
            }
        }
    }
}