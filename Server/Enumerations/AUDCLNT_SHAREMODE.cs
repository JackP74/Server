using System;

namespace CoreAudio.Enumerations
{
    /// <summary>
    /// Defines constants that indicate whether an audio stream will run in shared mode or in exclusive mode.
    /// </summary>
    /// <remarks>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/dd370790.aspx
    /// </remarks>
    public enum AUDCLNT_SHAREMODE
    {
        /// <summary>
        /// The audio stream will run in shared mode.
        /// </summary>
        AUDCLNT_SHAREMODE_SHARED = 0,

        /// <summary>
        /// The audio stream will run in exclusive mode.
        /// </summary>
        AUDCLNT_SHAREMODE_EXCLUSIVE = 1
    }
}
