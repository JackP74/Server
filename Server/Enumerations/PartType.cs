using System;
using System.Runtime.InteropServices;

namespace CoreAudio.Enumerations
{
    /// <summary>
    /// Defines constants that indicate whether a part in a device topology is a connector or subunit.
    /// </summary>
    /// <remarks>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/dd316559.aspx
    /// </remarks>
    public enum PartType
    {
        /// <summary>
        /// The part is a connector.
        /// </summary>
        Connector,

        /// <summary>
        /// The part is a subunit.
        /// </summary>
        Subunit 
    }
}
