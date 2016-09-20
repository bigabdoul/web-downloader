using System;

namespace Downloader.Core
{
    /// <summary>
    /// Represents a single internet resource to download.
    /// </summary>
    public class DownloadListItem : IComparable<DownloadListItem>
    {
        /// <summary>
        /// Initializes an instance of the <see cref="DownloadListItem"/> class.
        /// </summary>
        /// <param name="sourceUrl">The URL of the internet resource to download.</param>
        /// <param name="destinationFolder">The fully-qualified path to the destination folder. Can be null (Nothing in Visual Basic).</param>
        public DownloadListItem(string sourceUrl, string destinationFolder = null)
        {
            this.SourceUrl = sourceUrl.TrimEnd('/');
            this.DestinationFolder = destinationFolder;
        }

        /// <summary>
        /// Gets the URL of the internet resource to download.
        /// </summary>
        public string SourceUrl { get; private set; }

        /// <summary>
        /// Gets or sets the fully-qualified path to the download destination folder.
        /// </summary>
        public string DestinationFolder { get; set; }

        /// <summary>
        /// Returns the <see cref="SourceUrl"/> property value.
        /// </summary>
        /// <returns>A <see cref="System.String"/>.</returns>
        public override string ToString()
        {
            return this.SourceUrl;
        }

        /// <summary>
        /// Compares the <see cref="SourceUrl"/> property values of the current and specified <see cref="DownloadListItem"/> instances.
        /// </summary>
        /// <param name="other">The object to compare to.</param>
        /// <returns>
        /// A signed 32-bit integer that indicates whether this instance precedes (negative value), 
        /// follows (positive value), or appears at the same (zero value) order position as the parameter
        /// <paramref name="other"/>.
        /// </returns>
        public int CompareTo(DownloadListItem other)
        {
            return this.SourceUrl.CompareTo(other.SourceUrl);
        }
    }
}
