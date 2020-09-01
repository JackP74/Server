using System;
using System.Runtime.InteropServices;

namespace CoreAudio.Interfaces
{
    /// <summary>
    /// Provides access to a hardware automatic gain control (AGC).
    /// </summary>
    /// <remarks>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/dd370853.aspx
    /// </remarks>
    public partial interface IAudioAutoGainControl
    {
        /// <summary>
        /// Gets the current state (enabled or disabled) of the AGC.
        /// </summary>
        /// <param name="isEnabled">Receives the current AGC state.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int GetEnabled(
            [Out] [MarshalAs(UnmanagedType.Bool)] out bool isEnabled);

        /// <summary>
        /// Enables or disables the AGC.
        /// </summary>
        /// <param name="isEnabled">The new AGC state.</param>
        /// <param name="eventContext">A user context value that is passed to the notification callback.</param>
        /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
        [PreserveSig]
        int SetEnabled(
            [In] [MarshalAs(UnmanagedType.Bool)] bool isEnabled,
            [In, Optional] [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);
    }
}
