using System;

namespace CoreAudio.Constants
{
    /// <summary>
    /// Indicate the current state of an audio endpoint device. 
    /// </summary>
    /// <remarks>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/dd370823.aspx
    /// </remarks>
    public class DEVICE_STATE_XXX
    {
        /// <summary>
        /// The audio endpoint device is active.
        /// </summary>
        public const uint DEVICE_STATE_ACTIVE = 0x00000001;

        /// <summary>
        /// The audio endpoint device is disabled.
        /// </summary>
        public const uint DEVICE_STATE_DISABLED = 0x00000002;

        /// <summary>
        /// The audio endpoint device is not present because the audio adapter that connects to the endpoint device has been removed or disabled.
        /// </summary>
        public const uint DEVICE_STATE_NOTPRESENT = 0x00000004;

        /// <summary>
        /// The audio endpoint device is unplugged.
        /// </summary>
        public const uint DEVICE_STATE_UNPLUGGED = 0x00000008;

        /// <summary>
        /// Includes audio endpoint devices in all states.
        /// </summary>
        public const uint DEVICE_STATEMASK_ALL = 0x0000000F;
    }
}
