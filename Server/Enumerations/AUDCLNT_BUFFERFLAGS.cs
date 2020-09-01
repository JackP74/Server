using System;

namespace CoreAudio.Enumerations
{
    /// <summary>
    /// Defines flags that indicate the status of an audio endpoint buffer.
    /// </summary>
    /// <remarks>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/dd371458.aspx
    /// </remarks>
    [Flags]
    public enum AUDCLNT_BUFFERFLAGS
    {
        /// <summary>
        /// The data in the packet is not correlated with the previous packet's device position.
        /// </summary>
        AUDCLNT_BUFFERFLAGS_DATA_DISCONTINUITY = 0x1,

        /// <summary>
        /// Treat all of the data in the packet as silence and ignore the actual data values.
        /// </summary>
        AUDCLNT_BUFFERFLAGS_SILENT = 0x2,

        /// <summary>
        /// The time at which the device's stream position was recorded is uncertain.
        /// </summary>
        AUDCLNT_BUFFERFLAGS_TIMESTAMP_ERROR = 0x4
    }
}
