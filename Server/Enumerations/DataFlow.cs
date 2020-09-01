using System;
using System.Runtime.InteropServices;

namespace CoreAudio.Enumerations
{
    /// <summary>
    /// Indicates the data-flow direction of an audio stream through a connector.
    /// </summary>
    /// <remarks>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/dd370808.aspx
    /// </remarks>
    public enum DataFlow
    {
        /// <summary>
        /// The audio stream flows into the device through the connector.
        /// </summary>
        In,

        /// <summary>
        /// The audio stream flows out of the device through the connector.
        /// </summary>
        Out
    }
}
