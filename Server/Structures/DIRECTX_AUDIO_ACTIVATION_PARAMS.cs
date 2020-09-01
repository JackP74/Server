using System;
using System.Runtime.InteropServices;

namespace CoreAudio.Structures
{
    /// <summary>
    /// Specifies the initialization parameters for a DirectSound stream.
    /// </summary>
    /// <remarks>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/dd370826.aspx
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct DIRECTX_AUDIO_ACTIVATION_PARAMS
    {
        /// <summary>
        /// The size in bytes of the structure.
        /// </summary>
        int ParamsSize;

        /// <summary>
        /// A GUID value that identifies the audio session that the stream belongs to.
        /// </summary>
        Guid AudioSession;

        /// <summary>
        /// Stream-initialization flags.
        /// </summary>
        int AudioStreamFlags;
    }
}
