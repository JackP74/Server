using System;

namespace CoreAudio.Externals
{
    /// <summary>
    /// The STGM constants are flags that indicate conditions for creating and deleting the object and access modes for the object.
    /// </summary>
    /// <remarks>
    /// MSDN Reference: http://msdn.microsoft.com/en-us/library/aa380337.aspx
    /// Note: This item is external to CoreAudio API, and is defined in the Windows Structured Storage API.
    /// </remarks>
    public class STGM
    {
        /// <summary>
        /// Indicates that the object is read-only, meaning that modifications cannot be made.
        /// </summary>
        public const int STGM_READ = 0x00000000;

        /// <summary>
        /// Enables you to save changes to the object, but does not permit access to its data.
        /// </summary>
        public const int STGM_WRITE = 0x00000001;

        /// <summary>
        /// Enables access and modification of object data.
        /// </summary>
        public const int STGM_READWRITE = 0x00000002;
    }
}
