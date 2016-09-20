using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Downloader.Core
{
    public class DownloadManager : IDisposable
    {
        #region constructors

        public DownloadManager()
        {
        }

        public DownloadManager(ISynchronizeInvoke syncRoot)
        {
            this.SyncRoot = syncRoot;
        }

        #endregion

        #region member variables

        private static string[] EmptyStringArray = new string[0];
        private volatile bool _cancel;
        private volatile bool _busy;
        private volatile bool _disposed;
        private volatile int _progressValue;
        private float _minImageRatio;
        private int _filesSaved;
        private long _lastBytesSaved;
        private long _totalBytesDownloaded;

        private ISynchronizeInvoke _syncRoot;
        private Size _minImageSize = new Size(600, 600);
        private HttpClient _client = new HttpClient();
        private List<Exception> _errors = new List<Exception>();
        private StringCollection _skippedFiles = new StringCollection();
        private Dictionary<string, string[]> _downloadedUrls = new Dictionary<string, string[]>();

        #endregion

        #region events

        public event EventHandler Cancelled;
        public event EventHandler<DownloadEventArgs> ProgressBegin;
        public event EventHandler<DownloadEventArgs> ProgressStep;
        public event EventHandler<DownloadEventArgs> FileSaved;
        public event EventHandler<InternetConnectionStateChangedEventArgs> InternetConnectionStateChanged;

        #endregion

        #region properties

        public bool IsBusy { get { return _busy; } }
        public bool HasErrors { get { return _errors.Count > 0; } }

        public int TotalFilesSaved { get { return _filesSaved; } }

        public long LastBytesSaved { get { return _lastBytesSaved; } }

        public long TotalBytesDownloaded { get { return _totalBytesDownloaded; } }

        public ISynchronizeInvoke SyncRoot
        {
            get { return _syncRoot; }
            set { _syncRoot = value; }
        }

        public Size MinImageSize
        {
            get { return _minImageSize; }
            set { _minImageSize = value; }
        }

        public float MinImageAspectRatio
        {
            get { return _minImageRatio; }
            set { _minImageRatio = value; }
        }

        #endregion

        public IEnumerable<string> GetErrors()
        {
            foreach (Exception e in _errors)
            {
                yield return string.Format("{0}\n", e.Message).Trim();
            }
        }

        public async Task<string[]> GetImageLinks(string url)
        {
            string key = url.Trim().TrimEnd('/');

            if (_downloadedUrls.ContainsKey(key)) {
                return _downloadedUrls[key];
            }

            if (!_busy)
            {
                try
                {
                    _busy = true;
                    _cancel = false;
                    _errors.Clear();

                    var data = await _client.GetByteArrayAsync(url);
                    _totalBytesDownloaded += data.Length;

                    if (_cancel) return EmptyStringArray;

                    var doc = new HtmlDocument();
                    doc.LoadHtml(System.Text.Encoding.UTF8.GetString(data));

                    var nodes1 = doc.DocumentNode.Descendants("img").Where(e => e.Attributes.Contains("src")).ToArray();
                    var nodes2 = doc.DocumentNode.Descendants("a").Where(e => e.Attributes.Contains("href")).ToArray();

                    this.OnProgressBegin(nodes1.Length + nodes2.Length);
                    
                    var links = new List<string>();

                    if (nodes1.Length > 0) {
                        links.AddRange(this.GetAttributeValues(nodes1, "src"));
                    }
                    if (nodes2.Length > 0) {
                        links.AddRange(this.GetAttributeValues(nodes2, "href"));
                    }

                    MakeAbsoluteUri(links, new Uri(url));
                    
                    var result = links.ToArray();
                    _downloadedUrls.Add(key, result);

                    return result;
                }
                catch (HttpRequestException ex)
                {
                    this.LogError(ex);
                }
                catch (Exception ex)
                {
                    this.LogError(ex);
                }
                finally
                {
                    if (_cancel) this.OnCancelled();
                    _busy = false;
                }
            }
            return EmptyStringArray;
        }

        public async Task<int> DownloadImages(string folder, params string[] links)
        {
            return await DownloadImages(CancellationToken.None, PauseToken.None, folder, links);
        }

        public async Task<int> DownloadImages(CancellationToken cancelToken, string folder, params string[] links)
        {
            return await DownloadImages(cancelToken, PauseToken.None, folder, links);
        }

        public async Task<int> DownloadImages(CancellationToken cancelToken, PauseToken pauseToken, string folder, params string[] links)
        {
            if (!_busy)
            {
                try
                {
                    _busy = true;
                    _cancel = false;
                    _lastBytesSaved = 0L;

                    _errors.Clear();

                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }

                    int saved = 0, counter = 0, index = 1, max = links.Length;
                    int minW = _minImageSize.Width, minH = _minImageSize.Height;
                    
                    this.OnProgressBegin(max);

                    foreach (string url in links)
                    {
                        await pauseToken.WaitWhilePausedAsync();

                        this.OnProgressStep(string.Format("Downloading file {0} of {1}...", index++, max));

                        string name = ComputeUniqueName(url, folder);

                        if (_skippedFiles.Contains(name))
                        {
                            counter++;
                            continue;
                        }

                        if (File.Exists(name))
                        {
                            counter++;
                            _skippedFiles.Add(name);
                            continue;
                        }

                        var data = await _client.GetByteArrayAsync(url);
                        int dataLength = data.Length;
                        _totalBytesDownloaded += dataLength;

                        using(var img = Image.FromStream(new MemoryStream(data)))
                        {
                            int w = img.Width, h = img.Height;
                            bool shouldSave = w >= minW || h >= minH;
                            
                            if (shouldSave && _minImageRatio > 0) {
                                shouldSave = this.GetImageAspectRatio(w, h) >= _minImageRatio;
                            }

                            if (shouldSave)
                            {
                                try
                                {
                                    img.Save(name, img.RawFormat);
                                    saved++;
                                    counter++;
                                    _skippedFiles.Add(name);
                                    _lastBytesSaved += dataLength;
                                    this.OnFileSaved(name, url, data);
                                }
                                catch (Exception ex)
                                {
                                    _errors.Add(ex);
                                }
                            }
                            else
                            {
                                counter++;
                                _skippedFiles.Add(name);
                            } // endif
                        }

                        if (_cancel || cancelToken.IsCancellationRequested) break;
                    } // endforeach

                    _filesSaved += saved;
                    return counter;
                }
                catch (HttpRequestException ex)
                {
                    this.LogError(ex);
                }
                catch (Exception ex)
                {
                    this.LogError(ex);
                    return -1;
                }
                finally
                {
                    if (_cancel) this.OnCancelled();
                    _busy = false;
                }
            }
            return -2;
        }

        public bool Cancel()
        {
            if (_busy) {
                _cancel = true;
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                this.Cancel(5000);
                _client.Dispose();
                GC.SuppressFinalize(this);
                _disposed = true;
            }
        }

        #region helper methods

        private void Cancel(int millisecondsTimeout)
        {
            if (this.Cancel() && millisecondsTimeout != 0)
            {
                Task.Run(() =>
                {
                    while (_busy)
                    {
                        Thread.Sleep(100);
                    }
                }).Wait(millisecondsTimeout);
            }
        }

        private IEnumerable<string> GetAttributeValues(IEnumerable<HtmlNode> nodes, string attrName)
        {
            string empty = string.Empty;
            foreach (var n in nodes)
            {
                string val = n.Attributes[attrName].Value ?? empty;
                if (val.ToLower().Contains(".jpg"))
                {
                    int h = 0, w = 0;
                    if (n.Attributes.Contains("data-size"))
                    {
                        var parts = n.Attributes["data-size"].Value.Split('x');
                        bool success = parts.Length == 2 && int.TryParse(parts[0], out w) && int.TryParse(parts[1], out h);
                    }
                    else
                    {
                        bool success =
                            n.Attributes.Contains("height") && int.TryParse(n.Attributes["height"].Value, out h) &&
                            n.Attributes.Contains("width") && int.TryParse(n.Attributes["width"].Value, out w);
                    }

                    if (h > 0 && w > 0)
                    {
                        if ((h >= _minImageSize.Height || w >= _minImageSize.Width))
                        {
                            this.OnProgressStep();
                            yield return val;
                        }
                    }
                    else
                    {
                        this.OnProgressStep();
                        yield return val;
                    }
                }
                if (_cancel) break;
            }
        }

        public static string ComputeUniqueName(string url, string folder)
        {
            string name = url.Split('/').Last();

            name = string.Format("{0}_0x{1:x}{2}",
                Path.GetFileNameWithoutExtension(name),
                url.GetHashCode(),
                Path.GetExtension(name)
            );

            return Path.Combine(folder, name);
        }

        public static void MakeAbsoluteUri(IList<string> links, Uri baseUri)
        {
            for (int i = 0; i < links.Count; i++)
            {
                try
                {
                    string url = links[i];

                    if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    links[i] = new Uri(baseUri, url).AbsoluteUri;
                }
                catch
                {
                }
            }
        }

        [System.Runtime.InteropServices.DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int conn, int val);

        public enum InternetConnectionState
        {
            Modem = 0x1,
            Lan = 0x2,
            Proxy = 0x4,
            Ras = 0x10,
            Offline = 0x20,
            Configured = 0x40
        }

        public static bool InternetAvailable()
        {
            int conn;
            return InternetGetConnectedState(out conn, 0);
        }

        private void OnCancelled()
        {
            if (Cancelled != null)
            {
                try
                {
                    if (_syncRoot != null && _syncRoot.InvokeRequired)
                    {
                        _syncRoot.Invoke(Cancelled, new object[] { this, EventArgs.Empty });
                    }
                    else
                    {
                        Cancelled(this, EventArgs.Empty);
                    }
                }
                catch
                {
                }
            }
            _cancel = false;
        }

        private void OnProgressBegin(int maxSteps = 1)
        {
            _progressValue = 0;
            if (this.ProgressBegin != null)
            {
                try
                {
                    var e = new DownloadEventArgs() { ProgressMaximum = maxSteps };

                    if (_syncRoot != null && _syncRoot.InvokeRequired)
                    {
                        _syncRoot.Invoke(this.ProgressBegin, new object[] { this, e });
                    }
                    else
                    {
                        this.ProgressBegin(this, e);
                    }
                }
                catch
                {
                }
            }
        }

        private void OnProgressStep(string msg = null)
        {
            if (this.ProgressStep != null)
            {
                var e = new DownloadEventArgs() { ProgressValue = ++_progressValue, Message = msg };

                if (_syncRoot != null && _syncRoot.InvokeRequired)
                {
                    _syncRoot.Invoke(this.ProgressStep, new object[] { this, e });
                }
                else
                {
                    this.ProgressStep(this, e);
                }
            }
        }

        private void OnFileSaved(string name, string url, byte[] data, string msg = null)
        {
            if (this.FileSaved != null)
            {
                var e = new DownloadEventArgs() { Data = data, Message = msg, SavedFileName = name, Url = url, TotalBytes = _totalBytesDownloaded };

                if (_syncRoot != null && _syncRoot.InvokeRequired) {
                    _syncRoot.Invoke(this.FileSaved, new object[] { this, e });
                }
                else {
                    this.FileSaved(this, e);
                }
            }
        }

        private void OnInternetConnectionStateChanged(bool connected)
        {
            if (this.InternetConnectionStateChanged != null)
            {
                var e = new InternetConnectionStateChangedEventArgs(connected);
                
                if (_syncRoot != null && _syncRoot.InvokeRequired) {
                    _syncRoot.Invoke(this.InternetConnectionStateChanged, new object[] { this, e });
                }
                else {
                    this.InternetConnectionStateChanged(this, e);
                }
            }
        }

        private float GetImageAspectRatio(float width, float height)
        {
            if (width == 0F || height == 0F) return 0F;
            return width > height ? height / width : width / height;
        }

        private void LogError(Exception ex)
        {
            _errors.Add(ex.InnerException ?? ex);
        }

        #endregion helper methods
    }

    public sealed class DownloadEventArgs : CancelEventArgs
    {
        public DownloadEventArgs()
        {
        }

        public DownloadEventArgs(bool cancel) :base(cancel)
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

    public sealed class InternetConnectionStateChangedEventArgs : EventArgs
    {
        public InternetConnectionStateChangedEventArgs()
        {
        }

        public InternetConnectionStateChangedEventArgs(bool connected)
        {
            this.Connected = connected;
        }

        public bool Connected { get; private set; }
    }
}
