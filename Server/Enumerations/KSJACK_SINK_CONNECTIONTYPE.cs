using System;
using System.Runtime.InteropServices;

namespace CoreAudio.Enumerations
{
    /// <summary>
    /// Defines constants that specify the type of connection.
    /// </summary>
    /// <remarks>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/dd316547.aspx
    /// </remarks>
    public enum KSJACK_SINK_CONNECTIONTYPE
    {
        /// <summary>
        /// High-Definition Multimedia Interface (HDMI) connection.
        /// </summary>
        KSJACK_SINK_CONNECTIONTYPE_HDMI = 0,
        
        /// <summary>
        /// Display port.
        /// </summary>
        KSJACK_SINK_CONNECTIONTYPE_DISPLAYPORT = 1 
    }
}
