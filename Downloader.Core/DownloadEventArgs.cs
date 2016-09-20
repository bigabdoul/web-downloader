using System.ComponentModel;

namespace Downloader.Core
{
    /// <summary>
    /// Provides data for download-related events.
    /// </summary>
    public sealed class DownloadEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes an instance of the <see cref="DownloadEventArgs"/> class.
        /// </summary>
        public DownloadEventArgs()
        {
        }

        /// <summary>
        /// Initializes an instance of the <see cref="DownloadEventArgs"/> class with the property <see cref="CancelEventArgs.Cancel"/> set to the indicated value.
        /// </summary>
        /// <param name="cancel">true to cancel the event; otherwise, false.</param>
        public DownloadEventArgs(bool cancel) : base(cancel)
        {
        }

        /// <summary>
        /// Gets or sets the maximum value for a progress indicator.
        /// </summary>
        public int ProgressMaximum { get; set; }

        /// <summary>
        /// Gets or sets the actual progress value of the current event.
        /// </summary>
        public int ProgressValue { get; set; }

        /// <summary>
        /// Gets or sets the message associated with the current event.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the fully-qualified name, including the path, of the saved file.
        /// </summary>
        public string SavedFileName { get; set; }

        /// <summary>
        /// Gets or sets the URL of the internet resource being downloaded.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the binary content of the downloaded resource.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Gets or sets the total amount of bytes that have downloaded so far.
        /// </summary>
        public long TotalBytes { get; set; }
    }
}
