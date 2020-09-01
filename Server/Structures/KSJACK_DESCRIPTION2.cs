using System;
using System.Runtime.InteropServices;

namespace CoreAudio.Structures
{
    /// <summary>
    /// Describes an audio jack.
    /// </summary>
    /// <remarks>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/dd316545.aspx
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct KSJACK_DESCRIPTION2
    {
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 DeviceStateInfo;

        /// <summary>
        /// Stores the audio jack's capabilities.
        /// </summary>
        /// <remarks>
        /// From Ksmedia.h, the available flags for this are:
        /// 1. JACKDESC2_PRESENCE_DETECT_CAPABILITY (0x00000001)
        /// 2. JACKDESC2_DYNAMIC_FORMAT_CHANGE_CAPABILITY (0x00000002) 
        /// </remarks>
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 JackCapabilities;
    }
}
