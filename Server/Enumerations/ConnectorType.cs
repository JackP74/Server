using System;
using System.Runtime.InteropServices;

namespace CoreAudio.Enumerations
{
    /// <summary>
    /// Indicates the type of connection that a connector is part of.
    /// </summary>
    /// <remarks>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/dd370801.aspx
    /// </remarks>
    public enum ConnectorType
    {
        /// <summary>
        /// The connector is part of a connection of unknown type.
        /// </summary>
        Unknown_Connector,

        /// <summary>
        /// The connector is part of a physical connection to an auxiliary device that is installed inside the system chassis.
        /// </summary>
        Physical_Internal,

        /// <summary>
        /// The connector is part of a physical connection to an external device.
        /// </summary>
        Physical_External,

        /// <summary>
        /// The connector is part of a software-configured I/O connection.
        /// </summary>
        Software_IO,

        /// <summary>
        /// The connector is part of a permanent connection that is fixed and cannot be configured under software control.
        /// </summary>
        Software_Fixed,

        /// <summary>
        /// The connector is part of a connection to a network.
        /// </summary>
        Network 
    }
}
