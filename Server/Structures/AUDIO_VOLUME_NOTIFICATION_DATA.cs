using System;
using System.Runtime.InteropServices;

namespace CoreAudio.Structures
{
    /// <summary>
    /// Describes a change in the volume level or muting state of an audio endpoint device.
    /// </summary>
    /// <remarks>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/dd370799.aspx
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct AUDIO_VOLUME_NOTIFICATION_DATA
    {
        /// <summary>
        /// The user event context supplied during the change request.
        /// </summary>
        public Guid EventContext;

        /// <summary>
        /// Specifies whether the audio stream is currently muted.
        /// </summary>
        [MarshalAs(UnmanagedType.Bool)]
        public bool IsMuted;

        /// <summary>
        /// The current master volume level of the audio stream.
        /// </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float MasterVolume;

        /// <summary>
        /// Specifies the number of channels in the audio stream.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 ChannelCount;

        /// <summary>
        /// The volume level of each channel in the stream.
        /// </summary>
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.R4)]
        public float[] ChannelVolumes;
    }
}
