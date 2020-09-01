using System;

namespace CoreAudio.Constants
{
    /// <summary>
    /// Hardware support flags for an audio endpoint device.
    /// </summary>
    /// <remarks>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/dd370835.aspx
    /// </remarks>
    public class ENDPOINT_HARDWARE_SUPPORT_XXX
    {
        /// <summary>
        /// The audio endpoint device supports a hardware volume control.
        /// </summary>
        public const int ENDPOINT_HARDWARE_SUPPORT_VOLUME = 0x00000001;

        /// <summary>
        /// The audio endpoint device supports a hardware mute control.
        /// </summary>
        public const int ENDPOINT_HARDWARE_SUPPORT_MUTE = 0x00000002;

        /// <summary>
        /// The audio endpoint device supports a hardware peak meter.
        /// </summary>
        public const int ENDPOINT_HARDWARE_SUPPORT_METER = 0x00000004;
    }
}
