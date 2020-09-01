﻿using System;
using System.Runtime.InteropServices;

namespace CoreAudio.Structures
{
    /// <summary>
    /// Describes an audio jack.
    /// </summary>
    /// <remarks>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/dd316543.aspx
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct KSJACK_DESCRIPTION
    {
        /// <summary>
        /// Specifies the mapping of the two audio channels in a stereo jack to speaker positions.
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public Int32 ChannelMapping;

        /// <summary>
        /// The jack color.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 Color;

        /// <summary>
        /// The connection type.
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public Int32 ConnectionType;

        /// <summary>
        /// The geometric location of the jack.
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public Int32 GeoLocation;

        /// <summary>
        /// The general location of the jack.
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public Int32 GenLocation;

        /// <summary>
        /// The type of port represented by the jack.
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public Int32 PortConnection;

        /// <summary>
        /// Indicates whether an endpoint device is plugged into the jack, if supported by the adapter.
        /// </summary>
        [MarshalAs(UnmanagedType.Bool)]
        public bool IsConnected;
    }
}
