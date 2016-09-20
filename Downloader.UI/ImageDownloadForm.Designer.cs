namespace Downloader.UI
{
    partial class ImageDownloadForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            m_downloader.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.clbImageList = new System.Windows.Forms.CheckedListBox();
            this.btnOpenList = new System.Windows.Forms.Button();
            this.txtDestination = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtImageUrl = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tssItemCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssBytesSaved = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssTotalBytes = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssProgressList = new System.Windows.Forms.ToolStripProgressBar();
            this.tssProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.pnlImgSize = new System.Windows.Forms.Panel();
            this.nudRatio = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.btnApplySize = new System.Windows.Forms.Button();
            this.nudHeight = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.nudWidth = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.connectQualityView1 = new Downloader.UI.ConnectQualityView();
            this.connectionStateView1 = new Downloader.UI.ConnectionStateView();
            this.btnPause = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.pnlImgSize.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRatio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Web Site &URL:";
            // 
            // txtUrl
            // 
            this.txtUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUrl.Location = new System.Drawing.Point(15, 23);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(481, 20);
            this.txtUrl.TabIndex = 1;
            // 
            // clbImageList
            // 
            this.clbImageList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clbImageList.CheckOnClick = true;
            this.clbImageList.FormattingEnabled = true;
            this.clbImageList.Location = new System.Drawing.Point(12, 85);
            this.clbImageList.Name = "clbImageList";
            this.clbImageList.Size = new System.Drawing.Size(484, 184);
            this.clbImageList.TabIndex = 8;
            this.clbImageList.SelectedIndexChanged += new System.EventHandler(this.clbImageList_SelectedIndexChanged);
            // 
            // btnOpenList
            // 
            this.btnOpenList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenList.Location = new System.Drawing.Point(503, 50);
            this.btnOpenList.Name = "btnOpenList";
            this.btnOpenList.Size = new System.Drawing.Size(112, 23);
            this.btnOpenList.TabIndex = 4;
            this.btnOpenList.Text = "&Open URL List";
            this.btnOpenList.UseVisualStyleBackColor = true;
            this.btnOpenList.Click += new System.EventHandler(this.btnOpenList_Click);
            // 
            // txtDestination
            // 
            this.txtDestination.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDestination.Location = new System.Drawing.Point(15, 50);
            this.txtDestination.Name = "txtDestination";
            this.txtDestination.Size = new System.Drawing.Size(481, 20);
            this.txtDestination.TabIndex = 2;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(503, 21);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(112, 23);
            this.btnSearch.TabIndex = 3;
            this.btnSearch.Text = "&Download Images";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtImageUrl
            // 
            this.txtImageUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtImageUrl.Location = new System.Drawing.Point(12, 270);
            this.txtImageUrl.Name = "txtImageUrl";
            this.txtImageUrl.ReadOnly = true;
            this.txtImageUrl.Size = new System.Drawing.Size(484, 20);
            this.txtImageUrl.TabIndex = 9;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(503, 108);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(112, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssItemCount,
            this.tssStatus,
            this.tssBytesSaved,
            this.tssTotalBytes,
            this.tssProgressList,
            this.tssProgress});
            this.statusStrip1.Location = new System.Drawing.Point(0, 334);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(624, 22);
            this.statusStrip1.TabIndex = 12;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tssItemCount
            // 
            this.tssItemCount.Name = "tssItemCount";
            this.tssItemCount.Size = new System.Drawing.Size(40, 17);
            this.tssItemCount.Text = "Count";
            // 
            // tssStatus
            // 
            this.tssStatus.Name = "tssStatus";
            this.tssStatus.Size = new System.Drawing.Size(39, 17);
            this.tssStatus.Text = "Status";
            // 
            // tssBytesSaved
            // 
            this.tssBytesSaved.Name = "tssBytesSaved";
            this.tssBytesSaved.Size = new System.Drawing.Size(13, 17);
            this.tssBytesSaved.Text = "0";
            this.tssBytesSaved.ToolTipText = "Current Bytes Saved";
            // 
            // tssTotalBytes
            // 
            this.tssTotalBytes.Name = "tssTotalBytes";
            this.tssTotalBytes.Size = new System.Drawing.Size(13, 17);
            this.tssTotalBytes.Text = "0";
            this.tssTotalBytes.ToolTipText = "Total Bytes Downloaded";
            // 
            // tssProgressList
            // 
            this.tssProgressList.Name = "tssProgressList";
            this.tssProgressList.Size = new System.Drawing.Size(100, 16);
            this.tssProgressList.Visible = false;
            // 
            // tssProgress
            // 
            this.tssProgress.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tssProgress.Name = "tssProgress";
            this.tssProgress.Size = new System.Drawing.Size(100, 16);
            this.tssProgress.Visible = false;
            // 
            // pnlImgSize
            // 
            this.pnlImgSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlImgSize.Controls.Add(this.nudRatio);
            this.pnlImgSize.Controls.Add(this.label5);
            this.pnlImgSize.Controls.Add(this.btnApplySize);
            this.pnlImgSize.Controls.Add(this.nudHeight);
            this.pnlImgSize.Controls.Add(this.label4);
            this.pnlImgSize.Controls.Add(this.nudWidth);
            this.pnlImgSize.Controls.Add(this.label3);
            this.pnlImgSize.Controls.Add(this.label2);
            this.pnlImgSize.Location = new System.Drawing.Point(500, 141);
            this.pnlImgSize.Name = "pnlImgSize";
            this.pnlImgSize.Size = new System.Drawing.Size(121, 134);
            this.pnlImgSize.TabIndex = 7;
            // 
            // nudRatio
            // 
            this.nudRatio.Location = new System.Drawing.Point(60, 79);
            this.nudRatio.Name = "nudRatio";
            this.nudRatio.Size = new System.Drawing.Size(52, 20);
            this.nudRatio.TabIndex = 6;
            this.nudRatio.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudRatio.Value = new decimal(new int[] {
            40,
            0,
            0,
            0});
            this.nudRatio.ValueChanged += new System.EventHandler(this.ImageSize_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 83);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Ratio %:";
            // 
            // btnApplySize
            // 
            this.btnApplySize.Enabled = false;
            this.btnApplySize.Location = new System.Drawing.Point(14, 105);
            this.btnApplySize.Name = "btnApplySize";
            this.btnApplySize.Size = new System.Drawing.Size(98, 23);
            this.btnApplySize.TabIndex = 7;
            this.btnApplySize.Text = "Apply";
            this.btnApplySize.UseVisualStyleBackColor = true;
            this.btnApplySize.Click += new System.EventHandler(this.btnApplySize_Click);
            // 
            // nudHeight
            // 
            this.nudHeight.Location = new System.Drawing.Point(60, 53);
            this.nudHeight.Maximum = new decimal(new int[] {
            4096,
            0,
            0,
            0});
            this.nudHeight.Minimum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.nudHeight.Name = "nudHeight";
            this.nudHeight.Size = new System.Drawing.Size(52, 20);
            this.nudHeight.TabIndex = 4;
            this.nudHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudHeight.Value = new decimal(new int[] {
            550,
            0,
            0,
            0});
            this.nudHeight.ValueChanged += new System.EventHandler(this.ImageSize_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Height:";
            // 
            // nudWidth
            // 
            this.nudWidth.Location = new System.Drawing.Point(60, 27);
            this.nudWidth.Maximum = new decimal(new int[] {
            4096,
            0,
            0,
            0});
            this.nudWidth.Minimum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.nudWidth.Name = "nudWidth";
            this.nudWidth.Size = new System.Drawing.Size(52, 20);
            this.nudWidth.TabIndex = 2;
            this.nudWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudWidth.Value = new decimal(new int[] {
            550,
            0,
            0,
            0});
            this.nudWidth.ValueChanged += new System.EventHandler(this.ImageSize_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Width:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Minimum Image Size";
            // 
            // connectQualityView1
            // 
            this.connectQualityView1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.connectQualityView1.BackColor = System.Drawing.SystemColors.Window;
            this.connectQualityView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.connectQualityView1.Location = new System.Drawing.Point(252, 291);
            this.connectQualityView1.Margin = new System.Windows.Forms.Padding(4);
            this.connectQualityView1.Name = "connectQualityView1";
            this.connectQualityView1.Size = new System.Drawing.Size(233, 41);
            this.connectQualityView1.TabIndex = 11;
            // 
            // connectionStateView1
            // 
            this.connectionStateView1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.connectionStateView1.BackColor = System.Drawing.SystemColors.Window;
            this.connectionStateView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.connectionStateView1.Location = new System.Drawing.Point(12, 290);
            this.connectionStateView1.Margin = new System.Windows.Forms.Padding(4);
            this.connectionStateView1.Name = "connectionStateView1";
            this.connectionStateView1.Size = new System.Drawing.Size(233, 41);
            this.connectionStateView1.TabIndex = 10;
            // 
            // btnPause
            // 
            this.btnPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPause.Enabled = false;
            this.btnPause.Location = new System.Drawing.Point(502, 79);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(112, 23);
            this.btnPause.TabIndex = 5;
            this.btnPause.Text = "&Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // ImageDownloader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(624, 356);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.connectQualityView1);
            this.Controls.Add(this.connectionStateView1);
            this.Controls.Add(this.pnlImgSize);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtImageUrl);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnOpenList);
            this.Controls.Add(this.clbImageList);
            this.Controls.Add(this.txtDestination);
            this.Controls.Add(this.txtUrl);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(262, 317);
            this.Name = "ImageDownloader";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Image Downloader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ImageDownloader_FormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.pnlImgSize.ResumeLayout(false);
            this.pnlImgSize.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRatio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.CheckedListBox clbImageList;
        private System.Windows.Forms.Button btnOpenList;
        private System.Windows.Forms.TextBox txtDestination;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtImageUrl;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tssItemCount;
        private System.Windows.Forms.ToolStripStatusLabel tssStatus;
        private System.Windows.Forms.ToolStripProgressBar tssProgress;
        private System.Windows.Forms.Panel pnlImgSize;
        private System.Windows.Forms.NumericUpDown nudHeight;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudWidth;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnApplySize;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudRatio;
        private System.Windows.Forms.ToolStripStatusLabel tssBytesSaved;
        private System.Windows.Forms.ToolStripStatusLabel tssTotalBytes;
        private System.Windows.Forms.ToolStripProgressBar tssProgressList;
        private ConnectionStateView connectionStateView1;
        private ConnectQualityView connectQualityView1;
        private System.Windows.Forms.Button btnPause;
    }
}

