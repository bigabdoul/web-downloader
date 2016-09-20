using System;

namespace Downloader.Core
{
    public class DownloadListItem : IComparable<DownloadListItem>
    {
        public DownloadListItem(string sourceUrl, string destinationFolder = null)
        {
            this.SourceUrl = sourceUrl.TrimEnd('/');
            this.DestinationFolder = destinationFolder;
        }

        public string SourceUrl { get; private set; }

        public string DestinationFolder { get; set; }

        public override string ToString()
        {
            return this.SourceUrl;
        }

        public int CompareTo(DownloadListItem other)
        {
            return this.SourceUrl.CompareTo(other.SourceUrl);
        }
    }
}
