using System;

namespace CoreAudio.Constants
{
    /// <summary>
    /// Indicate special characteristics that a client can assign to an audio stream during the initialization of the stream.
    /// </summary>
    /// <remarks>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/dd370791.aspx
    /// </remarks>
    public class AUDCLNT_STREAMFLAGS_XXX
    {
        /// <summary>
        /// The audio stream will be a member of a cross-process audio session.
        /// </summary>
        public const UInt32 AUDCLNT_STREAMFLAGS_CROSSPROCESS = 0x00010000;

        /// <summary>
        /// The audio stream will operate in loopback mode.
        /// </summary>
        public const UInt32 AUDCLNT_STREAMFLAGS_LOOPBACK = 0x00020000;

        /// <summary>
        /// Processing of the audio buffer by the client will be event driven.
        /// </summary>
        public const UInt32 AUDCLNT_STREAMFLAGS_EVENTCALLBACK = 0x00040000;

        /// <summary>
        /// The volume and mute settings for an audio session will not persist across system restarts.
        /// </summary>
        public const UInt32 AUDCLNT_STREAMFLAGS_NOPERSIST = 0x00080000;

        /// <summary>
        /// The sample rate of the stream is adjusted to a rate specified by an application.
        /// </summary>
        public const UInt32 AUDCLNT_STREAMFLAGS_RATEADJUST = 0x00100000;
    }
}
