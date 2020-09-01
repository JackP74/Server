using System;

namespace CoreAudio.Constants
{
    /// <summary>
    /// Indicate characteristics of an audio session associated with the stream.
    /// </summary>
    /// <remarks>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/dd370789.aspx
    /// </remarks>
    public class AUDCLNT_SESSIONFLAGS_XXX
    {
        /// <summary>
        /// The session expires when there are no associated streams and owning session control objects holding references.
        /// </summary>
        public const int AUDCLNT_SESSIONFLAGS_EXPIREWHENUNOWNED = 0x10000000;

        /// <summary>
        /// The volume control is hidden in the volume mixer user interface when the audio session is created.
        /// </summary>
        public const int AUDCLNT_SESSIONFLAGS_DISPLAY_HIDE = 0x20000000;

        /// <summary>
        /// The volume control is hidden in the volume mixer user interface after the session expires.
        /// </summary>
        public const int AUDCLNT_SESSIONFLAGS_DISPLAY_HIDEWHENEXPIRED = 0x40000000;
    }
}
