using System;
using System.Runtime.InteropServices;

namespace CoreAudio.Structures
{
    /// <summary>
    /// Stores the video port identifier.
    /// </summary>
    /// <remarks>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/dd316552.aspx
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct LUID
    {
        /// <summary>
        /// LowPart of the video port identifier.
        /// </summary>
        int LowPart;

        /// <summary>
        /// HighPart of the video port identifier.
        /// </summary>
        long HighPart;
    }
}
