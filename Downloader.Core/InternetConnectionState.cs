namespace Downloader.Core
{
    /// <summary>
    /// Enumerations for internet connection states.
    /// </summary>
    public enum InternetConnectionState
    {
        /// <summary>
        /// Connected through a modem.
        /// </summary>
        Modem = 0x1,

        /// <summary>
        /// Connected through a Local Area Network (LAN).
        /// </summary>
        Lan = 0x2,

        /// <summary>
        /// Connected through a proxy.
        /// </summary>
        Proxy = 0x4,

        /// <summary>
        /// RAS connection.
        /// </summary>
        Ras = 0x10,

        /// <summary>
        /// No internet connection.
        /// </summary>
        Offline = 0x20,

        /// <summary>
        /// Connection configured.
        /// </summary>
        Configured = 0x40
    }
}
