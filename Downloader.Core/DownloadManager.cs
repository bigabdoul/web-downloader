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

using HtmlAgilityPack;

namespace Downloader.Core
{
    /// <summary>
    /// Represents an object that manages downloads of internet resources.
    /// </summary>
    public class DownloadManager : IDisposable
    {
        #region constructors

        /// <summary>
        /// Initializes an instance of the <see cref="DownloadManager"/> class.
        /// </summary>
        public DownloadManager()
        {
        }

        /// <summary>
        /// Initializes an instance of the <see cref="DownloadManager"/> class using an object used to synchronously execute event handler delegates.
        /// </summary>
        /// <param name="syncRoot">An object used to invoke synchronization delegates.</param>
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

        const char FSLASH = '/';
        const string HTTP = "http://";
        const string HTTPS = "https://";
        static readonly char[] IllegalFileNameChars = { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };

        #endregion

        #region events

        /// <summary>
        /// Event raised to notify handlers that an operation is being cancelled.
        /// </summary>
        public event EventHandler Cancelled;

        /// <summary>
        /// Event raised to notify handlers that progress indicators should be initialized.
        /// </summary>
        public event EventHandler<DownloadEventArgs> ProgressBegin;

        /// <summary>
        /// Event raised to notify handlers that progress value has been incremented.
        /// </summary>
        public event EventHandler<DownloadEventArgs> ProgressStep;

        /// <summary>
        /// Event raised to notify handlers that a downloaded resource has been saved.
        /// </summary>
        public event EventHandler<DownloadEventArgs> FileSaved;

        /// <summary>
        /// Event raised to notify handlers that a downloaded resource is requested to be saved.
        /// </summary>
        public event EventHandler<DownloadEventArgs> FileSaving;
        
        #endregion

        #region properties

        /// <summary>
        /// Gets a value that indicates whether the current download manager is working.
        /// </summary>
        public virtual bool IsBusy { get { return _busy; } }

        /// <summary>
        /// Indicates whether errors occured since the last download operation.
        /// </summary>
        public virtual bool HasErrors { get { return _errors.Count > 0; } }

        /// <summary>
        /// Gets the total number of files saved to the system.
        /// </summary>
        public virtual int TotalFilesSaved { get { return _filesSaved; } }

        /// <summary>
        /// Gets the number of bytes that have been saved during the last save operation.
        /// </summary>
        public virtual long LastBytesSaved { get { return _lastBytesSaved; } }

        /// <summary>
        /// Gets the total number of bytes downloaded since the creation of this <see cref="DownloadManager"/> instance.
        /// </summary>
        public virtual long TotalBytesDownloaded { get { return _totalBytesDownloaded; } }

        /// <summary>
        /// Gets or sets an object used to invoke synchronization delegates.
        /// </summary>
        public virtual ISynchronizeInvoke SyncRoot
        {
            get { return _syncRoot; }
            set { _syncRoot = value; }
        }

        /// <summary>
        /// Gets or sets the minimum size of images to download.
        /// </summary>
        public virtual Size MinImageSize
        {
            get { return _minImageSize; }
            set { _minImageSize = value; }
        }

        /// <summary>
        /// Gets or sets the minimum aspect ratio of images to download.
        /// </summary>
        public virtual float MinImageAspectRatio
        {
            get { return _minImageRatio; }
            set { _minImageRatio = value; }
        }

        #endregion

        #region public instance methods
        /// <summary>
        /// Downloads the document identified by <paramref name="url"/> and parses its content to find HTML tags that reference images.
        /// </summary>
        /// <param name="url">The URL of the document to download and parse.</param>
        /// <returns>A task that returns an array of string representing the parsed image references.</returns>
        public virtual async Task<string[]> GetImageLinks(string url)
        {
            string key = url.Trim().TrimEnd('/');

            if (_downloadedUrls.ContainsKey(key))
            {
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

                    if (nodes1.Length > 0)
                    {
                        links.AddRange(this.GetAttributeValues(nodes1, "src"));
                    }
                    if (nodes2.Length > 0)
                    {
                        links.AddRange(this.GetAttributeValues(nodes2, "href"));
                    }

                    MakeAbsoluteUri(links, new Uri(url));

                    var result = links.ToArray();
                    _downloadedUrls.Add(key, result);

                    return result;
                }
                catch (HttpRequestException ex)
                {
                    this.LogError(ex.InnerException ?? ex);
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

        /// <summary>
        /// Downloads to the specified folder the provided image references.
        /// </summary>
        /// <param name="folder">The fully-qualified path of the folder in which the downloaded images will be saved. If the directory does not exist, an attempt to create it will be made.</param>
        /// <param name="links">An array of URLs referencing the images to download.</param>
        /// <returns>A task that returns an integer that represents the number of images processed (not necessarily downloaded).</returns>
        public async Task<int> DownloadImages(string folder, params string[] links)
        {
            return await DownloadImages(CancellationToken.None, PauseToken.None, folder, links);
        }

        /// <summary>
        /// Downloads to the specified folder the provided image references using a cancellation token.
        /// </summary>
        /// <param name="cancelToken">A token used to cancel the task.</param>
        /// <param name="folder">The fully-qualified path of the folder in which the downloaded images will be saved. If the directory does not exist, an attempt to create it will be made.</param>
        /// <param name="links">An array of URLs referencing the images to download.</param>
        /// <returns>A task that returns an integer that represents the number of images processed (not necessarily downloaded).</returns>
        public async Task<int> DownloadImages(CancellationToken cancelToken, string folder, params string[] links)
        {
            return await DownloadImages(cancelToken, PauseToken.None, folder, links);
        }

        /// <summary>
        /// Downloads to the specified folder the provided image references using cancellation and pause tokens.
        /// </summary>
        /// <param name="cancelToken">A token used to cancel the task.</param>
        /// <param name="pauseToken">A token used to pause the task.</param>
        /// <param name="folder">The fully-qualified path of the folder in which the downloaded images will be saved. If the directory does not exist, an attempt to create it will be made.</param>
        /// <param name="links">An array of URLs referencing the images to download.</param>
        /// <returns>
        /// <para>
        /// A task that returns an integer which -when positive- represents the number of, including skipped, files that have been
        /// processed (not necessarily downloaded), or -when negative- the error code of the exception category that occured.
        /// </para>
        /// <para>-3 for a <see cref="AggregateException"/>.</para>
        /// <para>-2 for a <see cref="HttpRequestException"/>.</para>
        /// <para>-1 for a general <see cref="Exception"/>.</para>
        /// <para> 0 when no file has been processed.</para>
        /// </returns>
        /// <remarks>
        /// <para>This method skips images that have already been downloaded and saved to the specified destination folder.</para>
        /// <para>It also skips images that are not compliant with the current minimum size and, eventually, minimum aspect ratio (if stricly positive).</para>
        /// <para>The return value of the method is an integer that indicates the number of links processed, including any skipped files.</para>
        /// </remarks>
        public virtual async Task<int> DownloadImages(CancellationToken cancelToken, PauseToken pauseToken, string folder, params string[] links)
        {
            if (NewDownload(folder, links, links.Length))
            {
                try
                {
                    int saved = 0, minW = _minImageSize.Width, minH = _minImageSize.Height;

                    int result = await DownloadCore(cancelToken, pauseToken, folder, links, (data, url, computedName) =>
                    {
                        bool processed = false;

                        using (var img = Image.FromStream(new MemoryStream(data)))
                        {
                            int w = img.Width, h = img.Height;
                            bool shouldSave = w >= minW || h >= minH;

                            if (shouldSave && _minImageRatio > 0)
                            {
                                shouldSave = this.GetImageAspectRatio(w, h) >= _minImageRatio;
                            }

                            if (shouldSave)
                            {
                                try
                                {
                                    int ioutcome = OnFileSaving(data, url, computedName);
                                    if (ioutcome == 0)
                                    {
                                        img.Save(computedName, img.RawFormat);
                                        _lastBytesSaved += data.Length;
                                        this.OnFileSaved(computedName, url, data);
                                    }
                                    if (ioutcome == 0 || ioutcome == 1)
                                    {
                                        processed = true;
                                        saved++;
                                        _skippedFiles.Add(computedName);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _errors.Add(ex);
                                }
                            }
                            else
                            {
                                processed = true;
                                _skippedFiles.Add(computedName);
                            } // endif
                        } // endusing

                        return processed;
                    });

                    _filesSaved += saved;
                    return result;
                }
                catch (AggregateException ex)
                {
                    foreach (var e in ex.InnerExceptions)
                    {
                        if (!(e is ThreadAbortException))
                        {
                            LogError(e);
                        }
                    }
                    return -3;
                }
                catch (HttpRequestException ex)
                {
                    this.LogError(ex.InnerException ?? ex);
                    return -2;
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
            return 0;
        }

        /// <summary>
        /// Downloads to the specified folder the provided file references.
        /// </summary>
        /// <param name="folder">The fully-qualified path of the folder in which the downloaded files will be saved. If the directory does not exist, an attempt to create it will be made.</param>
        /// <param name="links">An array of URLs referencing the files to download.</param>
        /// <returns>A task that returns an integer that represents the number of files processed (not necessarily downloaded).</returns>
        public async Task<int> DownloadFiles(string folder, params string[] links)
        {
            return await DownloadFiles(CancellationToken.None, PauseToken.None, folder, links);
        }

        /// <summary>
        /// Downloads to the specified folder the provided file references using a cancellation token.
        /// </summary>
        /// <param name="cancelToken">A token used to cancel the task.</param>
        /// <param name="folder">The fully-qualified path of the folder in which the downloaded files will be saved. If the directory does not exist, an attempt to create it will be made.</param>
        /// <param name="links">An array of URLs referencing the files to download.</param>
        /// <returns>A task that returns an integer that represents the number of files processed (not necessarily downloaded).</returns>
        public async Task<int> DownloadFiles(CancellationToken cancelToken, string folder, params string[] links)
        {
            return await DownloadFiles(cancelToken, PauseToken.None, folder, links);
        }

        /// <summary>
        /// Downloads to the specified folder the provided file references using cancellation and pause tokens.
        /// </summary>
        /// <param name="cancelToken">A token used to cancel the task.</param>
        /// <param name="pauseToken">A token used to pause the task.</param>
        /// <param name="folder">The fully-qualified path of the folder in which the downloaded files will be saved. If the directory does not exist, an attempt to create it will be made.</param>
        /// <param name="links">An array of URLs referencing the files to download.</param>
        /// <returns>
        /// <para>
        /// A task that returns an integer which -when positive- represents the number of, including skipped, files that have been
        /// processed (not necessarily downloaded), or -when negative- the error code of the exception category that occured.
        /// </para>
        /// <para>-3 for a <see cref="AggregateException"/>.</para>
        /// <para>-2 for a <see cref="HttpRequestException"/>.</para>
        /// <para>-1 for a general <see cref="Exception"/>.</para>
        /// <para> 0 when no file has been processed.</para>
        /// </returns>
        public virtual async Task<int> DownloadFiles(CancellationToken cancelToken, PauseToken pauseToken, string folder, params string[] links)
        {
            if (NewDownload(folder, links, links.Length))
            {
                try
                {
                    int saved = 0;

                    int result = await DownloadCore(cancelToken, pauseToken, folder, links, (data, url, computedName) =>
                    {
                        bool processed = false;
                        try
                        {
                            int ioutcome = OnFileSaving(data, url, computedName);
                            if (ioutcome == 0)
                            {
                                File.WriteAllBytes(computedName, data);
                                _lastBytesSaved += data.Length;
                                this.OnFileSaved(computedName, url, data);
                            }
                            if (ioutcome == 0 || ioutcome == 1)
                            {
                                processed = true;
                                saved++;
                                _skippedFiles.Add(computedName);
                            }
                        }
                        catch (Exception ex)
                        {
                            _errors.Add(ex);
                        }
                        return processed;
                    });

                    _filesSaved += saved;
                    return result;
                }
                catch (AggregateException ex)
                {
                    foreach (var e in ex.InnerExceptions)
                    {
                        if (!(e is ThreadAbortException))
                        {
                            LogError(e);
                        }
                    }
                    return -3;
                }
                catch (HttpRequestException ex)
                {
                    this.LogError(ex.InnerException ?? ex);
                    return -2;
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
            return 0;
        }

        /// <summary>
        /// Cancels the current download operation.
        /// </summary>
        /// <returns>true if the download manager is busy; otherwise, false.</returns>
        public virtual bool Cancel()
        {
            if (_busy)
            {
                _cancel = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns a collection string messages that represents the errors that occured.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<string> GetErrors()
        {
            foreach (Exception e in _errors)
            {
                yield return string.Format("{0}\n", e.Message).Trim();
            }
        }

        /// <summary>
        /// Releases all managed and unmanaged resources used by the current <see cref="DownloadManager"/> instance.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(false);
        }
        #endregion

        #region public static methods

        /// <summary>
        /// Generates a unique file name derived from the specified <paramref name="url"/>
        /// and combines it into a path with the provided <paramref name="folder"/>.
        /// </summary>
        /// <param name="url">The URL of the resource for which to generate </param>
        /// <param name="folder">The destination folder of the file.</param>
        /// <returns></returns>
        public static string ComputeUniqueName(string url, string folder)
        {
            string name = url.TrimEnd(FSLASH).Split(FSLASH).Last();

            name = string.Format("{0}_0x{1:x}{2}",
                Path.GetFileNameWithoutExtension(name),
                url.GetHashCode(),
                Path.GetExtension(name)
            );

            if (name.IndexOfAny(IllegalFileNameChars) > -1)
            {
                var b = new System.Text.StringBuilder(name);
                foreach (var c in name) {
                    if (IllegalFileNameChars.Contains(c)) {
                        b.Replace(c.ToString(), string.Empty);
                    }
                }
                name = b.ToString();
                b.Clear();
            }

            return Path.Combine(folder, name);
        }

        /// <summary>
        /// Makes sure that all provided links are absolute (and not relative) URIs.
        /// </summary>
        /// <param name="links">A list of links to make absolute URI.</param>
        /// <param name="baseUri">The base URI to use for any relative link found in the list.</param>
        public static void MakeAbsoluteUri(IList<string> links, Uri baseUri)
        {
            for (int i = 0; i < links.Count; i++)
            {
                try
                {
                    string url = links[i];

                    if (url.StartsWith(HTTP, StringComparison.OrdinalIgnoreCase) || url.StartsWith(HTTPS, StringComparison.OrdinalIgnoreCase))
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

        /// <summary>
        /// Checks whether the local system is actually connected to the internet.
        /// </summary>
        /// <returns></returns>
        public static bool InternetAvailable()
        {
            int conn;
            return InternetGetConnectedState(out conn, 0);
        }

        #endregion

        #region protected methods

        /// <summary>
        /// Loops through all provided links and, based on the given folder, determines for each 
        /// URL found in the <paramref name="links"/> array whether a download is required.
        /// </summary>
        /// <param name="cancelToken">A token used to cancel the task.</param>
        /// <param name="pauseToken">A token used to pause the task.</param>
        /// <param name="folder">The fully-qualified path of the folder in which the downloaded files will presumably be saved. This parameter is used only to compute a unique file name for each provided URL.</param>
        /// <param name="links">An array of URLs referencing the files to download.</param>
        /// <param name="saveDataCallback">A callback delegate that executes the persistence logic for the data downloaded.</param>
        /// <param name="skipRequiredCallback">
        /// An optional callback delegate that peforms custom checks on whether to skip the resource identified 
        /// by the computed name or not. If this argument is omitted, the default behavior is to skip any file 
        /// that matches the computed name either in the current list of skipped files, or in the local file system.
        /// </param>
        /// <returns>An integer that represents the number of, including skipped, URLs that have been processed.</returns>
        protected virtual async Task<int> DownloadCore(CancellationToken cancelToken, PauseToken pauseToken, string folder, string[] links, Func<byte[], string, string, bool> saveDataCallback, Func<string, string, bool> skipRequiredCallback = null)
        {
            int counter = 0, index = 1, max = links.Length;

            foreach (string url in links)
            {
                await pauseToken.WaitWhilePausedAsync();

                this.OnProgressStep(string.Format("Downloading file {0} of {1}...", index++, max));

                string name = ComputeUniqueName(url, folder);

                if (skipRequiredCallback != null) {
                    if (skipRequiredCallback(url, name)) {
                        if (!_skippedFiles.Contains(name)) _skippedFiles.Add(name);
                        counter++;
                        continue;
                    }
                }
                else if (_skippedFiles.Contains(name)) {
                    counter++;
                    continue;
                }
                else if (File.Exists(name)) {
                    counter++;
                    _skippedFiles.Add(name);
                    continue;
                }
                
                // download the resource as a byte array
                var data = await _client.GetByteArrayAsync(url);

                _totalBytesDownloaded += data.Length;

                // invoke persistence policy
                if (saveDataCallback(data, url, name))
                    counter++;

                if (_cancel || cancelToken.IsCancellationRequested) break;
            } // endforeach

            return counter;
        }
        
        /// <summary>
        /// Cancels the current operation with an optional timeout.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait before timing out.</param>
        protected virtual void Cancel(int millisecondsTimeout)
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

        /// <summary>
        /// Fires the <see cref="Cancelled"/> event.
        /// </summary>
        protected virtual void OnCancelled()
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

        /// <summary>
        /// Fires the <see cref="ProgressBegin"/> event.
        /// </summary>
        /// <param name="maxSteps">The number of maximum steps required for the current operation to complete. This argument is optional.</param>
        protected virtual void OnProgressBegin(int maxSteps = 1)
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

        /// <summary>
        /// Fires the <see cref="ProgressStep"/> event.
        /// </summary>
        /// <param name="msg">The message to include in the event data. This argument is optional.</param>
        protected virtual void OnProgressStep(string msg = null)
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

        /// <summary>
        /// Fires the <see cref="FileSaving"/> event.
        /// </summary>
        /// <param name="data">The downloaded content that should be saved.</param>
        /// <param name="url">The URL of the downloaded data.</param>
        /// <param name="name">The name of the file to save.</param>
        /// <param name="msg">The message to include. This argument is optional.</param>
        /// <returns>
        ///  0, if the event is not cancelled;
        ///  1, if the event is cancelled but was handled by the user;
        /// -1, if the event is cancelled and was not handled by the user.
        /// </returns>
        protected virtual int OnFileSaving(byte[] data, string url, string name, string msg = null)
        {
            if (FileSaving != null)
            {
                var e = new DownloadEventArgs(false) { Data = data, Message = msg, SavedFileName = name, Url = url, TotalBytes = _totalBytesDownloaded };

                if (_syncRoot != null && _syncRoot.InvokeRequired)
                {
                    _syncRoot.Invoke(this.FileSaving, new object[] { this, e });
                }
                else
                {
                    this.FileSaving(this, e);
                }
                if (e.Cancel)
                {
                    return (e.Handled) ? 1 : -1;
                }
            }
            return 0;
        }

        /// <summary>
        /// Fires the <see cref="FileSaved"/> event.
        /// </summary>
        /// <param name="name">The name of the saved file.</param>
        /// <param name="url">The URL of the saved file.</param>
        /// <param name="data">The downloaded content that was saved into the file.</param>
        /// <param name="msg">The message to include. This argument is optional.</param>
        protected virtual void OnFileSaved(string name, string url, byte[] data, string msg = null)
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
        
        /// <summary>
        /// Adds the specified exception to the internal exceptions list.
        /// </summary>
        /// <param name="ex">
        /// The error that occured. If the exception's InnerException property is set
        /// then that exception is added; otherwise, the specified exception is added.
        /// </param>
        protected virtual void LogError(Exception ex)
        {
            _errors.Add(ex.InnerException ?? ex);
        }

        /// <summary>
        /// Releases resources used by the current <see cref="DownloadManager"/> instance.
        /// </summary>
        /// <param name="disposing">true to release only managed resources; false to release both managed and unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    this.Cancel(5000);
                    _client.Dispose();
                }
                GC.SuppressFinalize(this);
                _disposed = true;
            }
        }

        #endregion

        #region private helper methods

        private bool NewDownload(string folder, string[] links, int maxSteps)
        {
            lock (this)
            {
                if (_busy) {
                    return false;
                }

                _busy = true;
                _cancel = false;
                _lastBytesSaved = 0L;

                _errors.Clear();

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                this.OnProgressBegin(maxSteps);
                return true;
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
        
        private float GetImageAspectRatio(float width, float height)
        {
            if (width == 0F || height == 0F) return 0F;
            return width > height ? height / width : width / height;
        }

        #endregion
    }
}
