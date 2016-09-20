using Downloader.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Downloader.UI
{
    public partial class ImageDownloadForm : Form
    {
        bool m_cancel;
        string[] m_links;
        const string FOLDER = "[FOLDER]";
        const int SIZE_KB = 0x400;
        const double SIZE_MB = 0x100000;

        DownloadManager m_downloader;
        DownloadList m_downloadList = new DownloadList();

        CancellationTokenSource m_cancelTokenSource;
        PauseTokenSource m_pauseTokenSource;

        public ImageDownloadForm()
        {
            InitializeComponent();
            
            m_downloader = new DownloadManager(this);
            
            this.ApplyImageSize();

            m_downloader.Cancelled += (s, e) =>
            {
                this.btnCancel.Enabled = false;
            };

            m_downloader.ProgressBegin += (s, e) =>
            {
                this.tssProgress.Maximum = e.ProgressMaximum;
                this.tssProgress.Value = 0;
                this.tssProgress.Visible = true;
            };

            m_downloader.ProgressStep += (s, e) => 
            {
                this.tssProgress.Value = e.ProgressValue;
                this.tssStatus.Text = e.Message;
            };

            m_downloader.FileSaved += (s, e) =>
            {
                this.ShowDownloadStatus(e);
            };
        }

        private void btnOpenList_Click(object sender, EventArgs e)
        {
            try
            {
                var ofd = new OpenFileDialog();
                ofd.Filter = "Text Files|*.txt|All Files|*.*";
                ofd.CheckFileExists = true;

                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var data = File.ReadAllText(ofd.FileName) ?? string.Empty;

                    if (!string.IsNullOrWhiteSpace(data))
                    {
                        var lines = data.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                            .Where(s => !s.StartsWith("//") && !s.StartsWith("#"))
                            .ToArray();

                        if (lines.Length > 0) {
                            m_downloadList.Clear();
                            string folder = null;
                            int foldIdx = FOLDER.Length;

                            for (int i = 0; i < lines.Length; i++) {
                                string key = lines[i].TrimEnd('/');

                                if (key.StartsWith(FOLDER)) {
                                    folder = key.Substring(foldIdx).Trim();
                                    continue;
                                }

                                if (!m_downloadList.Contains(key))
                                {
                                    m_downloadList.Add(new DownloadListItem(key, folder));
                                }
                            }
                            this.tssStatus.Text = "URLs found: " + m_downloadList.Count;
                        }
                    }
                    else {
                        MessageBox.Show("The file does not contain any URL to download.");
                    }
                }
            }
            catch (Exception ex)
            {
                this.ShowError(ex);
            }
        }

        private void clbImageList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.clbImageList.SelectedIndex >= 0)
            {
                this.txtImageUrl.Text = this.clbImageList.SelectedItem.ToString();
            }
            else
            {
                this.txtImageUrl.Clear();
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            bool paused = m_pauseTokenSource.IsPaused = !m_pauseTokenSource.IsPaused;
            this.SetPauseState(paused);
        }

        private void SetPauseState(bool paused)
        {
            if (paused) {
                this.tssItemCount.Text = "Paused";
                this.btnPause.Text = "&Resume";
            } else {
                this.btnPause.Text = "&Pause";
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.btnCancel.Enabled = false;
            this.m_downloader.Cancel();
            m_cancel = true;
            if (m_pauseTokenSource.IsPaused) {
                m_pauseTokenSource.IsPaused = false;
                this.SetPauseState(false);
            }
            m_cancelTokenSource.Cancel();
        }

        private void btnApplySize_Click(object sender, EventArgs e)
        {
            this.ApplyImageSize();
        }

        private void ImageSize_ValueChanged(object sender, EventArgs e)
        {
            this.btnApplySize.Enabled = true;
        }

        private void ImageDownloader_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.m_downloader.IsBusy)
            {
                var resp = MessageBox.Show("The downloader is busy. Do you want to cancel the task and quit?", "Downloading", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (resp == DialogResult.Yes) {
                    this.m_downloader.Cancel();
                }
                else {
                    e.Cancel = true;
                }
            }
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            int listCount = m_downloadList.Count;

            if (listCount == 0 && this.txtUrl.Text.Trim().Length == 0) {
                this.ShowMessage("Please type a URL for which to download resources or open a URL list file.");
                return;
            }

            if (this.txtDestination.Text.Trim().Length == 0) {
                bool nodest = m_downloadList.All(dli => string.IsNullOrWhiteSpace(dli.DestinationFolder));
                if (nodest) {
                    this.ShowMessage("Please enter a destination folder for downloads.");
                    return;
                }
            }

            m_cancel = false;
            m_pauseTokenSource = new PauseTokenSource();
            m_cancelTokenSource = new CancellationTokenSource();

            this.DownloadUpdateControls(false);

            try
            {
                if (listCount > 0)
                {
                    try
                    {
                        this.tssProgressList.Value = 0;
                        this.tssProgressList.Maximum = listCount;
                        this.tssProgressList.Visible = true;
                        int total = listCount;

                        while (listCount > 0)
                        {
                            var item = m_downloadList[0];
                            this.tssProgressList.Value += 1;
                            this.tssItemCount.Text = string.Format("Processing URL... {0} of {1} | ", this.tssProgressList.Value, total);

                            await this.SearchImages(item.SourceUrl, m_cancelTokenSource.Token, m_pauseTokenSource.Token, item.DestinationFolder);

                            m_downloadList.Remove(item);
                            listCount--;

                            if (m_cancel) break;
                        }
                    }
                    catch (Exception ex)
                    {
                        this.ShowError(ex);
                    }

                    this.tssProgressList.Visible = false;
                    this.tssItemCount.Text = string.Format("Images found: {0}", this.clbImageList.Items.Count);
                }
                else
                {
                    await this.SearchImages(this.txtUrl.Text.Trim(), m_cancelTokenSource.Token, m_pauseTokenSource.Token);
                }
            }
            catch (Exception)
            {
            }

            this.DownloadUpdateControls(true);
        }

        #region helper methods

        private async Task SearchImages(string url, CancellationToken cancelToken, PauseToken pauseToken, string folder = null)
        {
            string[] links;

            if (!(url.StartsWith("http://") || url.StartsWith("https://"))) {
                url = "http://" + url;
            }

            this.txtUrl.Text = url;
            int attempts = 0;

            while (true)
            {
                await pauseToken.WaitWhilePausedAsync();

                if (m_cancel) return;

                links = await m_downloader.GetImageLinks(url);

                if (links.Length == 0 && m_downloader.HasErrors) {
                    this.ShowLastErrors();
                    if (++attempts == 10) {
                        break;
                    }
                }
                else {
                    break;
                }
            }

            m_links = links;

            this.clbImageList.BeginUpdate();
            this.clbImageList.Items.Clear();
            this.clbImageList.Items.AddRange(m_links);
            this.clbImageList.EndUpdate();
            this.tssItemCount.Text = string.Format("Images found: {0}", this.clbImageList.Items.Count);

            this.ShowLastErrors();

            if (m_links.Length > 0)
                await this.DownloadImages(cancelToken, pauseToken, folder);
        }

        private async Task<bool> DownloadImages(CancellationToken cancelToken, PauseToken pauseToken, string folder = null)
        {
            folder = folder ?? this.txtDestination.Text.Trim();

            if (string.IsNullOrWhiteSpace(folder))
            {
                return false;
            }

            this.txtDestination.Text = folder;
            int processed = 0, len = m_links.Length;

            while (processed < len)
            {
                int result = await m_downloader.DownloadImages(cancelToken, pauseToken, folder, m_links);

                if (result > 0) {
                    processed += result;
                }

                if (m_cancel) break;
                await pauseToken.WaitWhilePausedAsync();
            }

            this.ShowLastErrors();
            this.ShowDownloadStatus();

            if (this.m_downloader.TotalFilesSaved > 0) {
                this.tssStatus.Text = " | " + string.Format("{0} images were downloaded.", this.m_downloader.TotalFilesSaved);
            }

            return processed > 0;
        }

        private void DownloadUpdateControls(bool finished)
        {
            this.btnSearch.Enabled = finished;
            this.btnOpenList.Enabled = finished;
            this.pnlImgSize.Enabled = finished;
            this.btnCancel.Enabled = !finished;
            this.btnPause.Enabled = !finished;
            this.txtDestination.ReadOnly = !finished;
            this.txtUrl.ReadOnly = !finished;
            this.tssProgress.Visible = !finished;
        }

        private void ApplyImageSize()
        {
            this.m_downloader.MinImageSize = new Size(Convert.ToInt32(this.nudWidth.Value), Convert.ToInt32(this.nudHeight.Value));
            this.m_downloader.MinImageAspectRatio = Convert.ToSingle(this.nudRatio.Value / 100M);
        }

        private void ShowLastErrors()
        {
            string msg = string.Join("\n", m_downloader.GetErrors());
            this.tssStatus.Text = msg;
        }

        private void ShowDownloadStatus(DownloadEventArgs args = null)
        {
            double saved = m_downloader.LastBytesSaved;
            bool mb = saved >= SIZE_MB;
            double divider = mb ? SIZE_MB : SIZE_KB;
            string fmt = " | Saved: " + (mb ? "{0:N2} MB" : "{0:N0} KB");

            this.tssBytesSaved.Text = string.Format(fmt, saved / divider);
            this.tssTotalBytes.Text = string.Format(" | Total Downloaded: {0:N2} MB", (double)m_downloader.TotalBytesDownloaded / SIZE_MB);

            //if (args != null && args.Data != null)
            //{
            //    string name = System.IO.Path.GetFileName(args.SavedFileName);
            //    this.tssStatus.Text = string.Format("{0}: {1}", name, args.Data.Length / SIZE_KB);
            //}
        }

        private void ShowError(Exception ex, MessageBoxIcon icon = MessageBoxIcon.Error)
        {
            this.ShowMessage(ex.Message, icon, ex.Source);
        }

        private void ShowMessage(string message, MessageBoxIcon icon = MessageBoxIcon.Warning, string title = null)
        {
            MessageBox.Show(message, title ?? this.Text, MessageBoxButtons.OK, icon);
        }

        #endregion helper methods
    }
}
