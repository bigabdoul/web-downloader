using System.ComponentModel;

namespace Downloader.Core
{
    public sealed class DownloadEventArgs : CancelEventArgs
    {
        public DownloadEventArgs()
        {
        }

        public DownloadEventArgs(bool cancel) : base(cancel)
        {
        }

        public int ProgressMaximum { get; set; }
        public int ProgressValue { get; set; }

        public string Message { get; set; }

        public string SavedFileName { get; set; }

        public string Url { get; set; }

        public byte[] Data { get; set; }

        public long TotalBytes { get; set; }
    }
}
