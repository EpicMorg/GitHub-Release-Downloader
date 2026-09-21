namespace GitHub_Release_Downloader
{
    partial class FrmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            gbUrl = new GroupBox();
            txtUrl = new TextBox();
            gbPath = new GroupBox();
            btnBrowse = new Button();
            txtPath = new TextBox();
            gbProgress = new GroupBox();
            pbDownload = new ProgressBar();
            btnDownload = new Button();
            txtLog = new TextBox();
            fbdTarget = new FolderBrowserDialog();
            gbUrl.SuspendLayout();
            gbPath.SuspendLayout();
            gbProgress.SuspendLayout();
            SuspendLayout();
            // 
            // gbUrl
            // 
            gbUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbUrl.Controls.Add(txtUrl);
            gbUrl.Location = new Point(12, 12);
            gbUrl.Name = "gbUrl";
            gbUrl.Size = new Size(638, 65);
            gbUrl.TabIndex = 0;
            gbUrl.TabStop = false;
            gbUrl.Text = "URL";
            // 
            // txtUrl
            // 
            txtUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtUrl.Location = new Point(6, 22);
            txtUrl.Name = "txtUrl";
            txtUrl.Size = new Size(626, 23);
            txtUrl.TabIndex = 0;
            // 
            // gbPath
            // 
            gbPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbPath.Controls.Add(btnBrowse);
            gbPath.Controls.Add(txtPath);
            gbPath.Location = new Point(12, 83);
            gbPath.Name = "gbPath";
            gbPath.Size = new Size(638, 65);
            gbPath.TabIndex = 1;
            gbPath.TabStop = false;
            gbPath.Text = "Path";
            // 
            // btnBrowse
            // 
            btnBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowse.Location = new Point(557, 22);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(75, 23);
            btnBrowse.TabIndex = 1;
            btnBrowse.Text = "Browse";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // txtPath
            // 
            txtPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtPath.Location = new Point(6, 22);
            txtPath.Name = "txtPath";
            txtPath.Size = new Size(545, 23);
            txtPath.TabIndex = 0;
            // 
            // gbProgress
            // 
            gbProgress.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gbProgress.Controls.Add(pbDownload);
            gbProgress.Controls.Add(btnDownload);
            gbProgress.Controls.Add(txtLog);
            gbProgress.Location = new Point(12, 154);
            gbProgress.Name = "gbProgress";
            gbProgress.Size = new Size(638, 218);
            gbProgress.TabIndex = 2;
            gbProgress.TabStop = false;
            gbProgress.Text = "Progress";
            // 
            // pbDownload
            // 
            pbDownload.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pbDownload.Location = new Point(6, 189);
            pbDownload.Name = "pbDownload";
            pbDownload.Size = new Size(545, 23);
            pbDownload.TabIndex = 2;
            // 
            // btnDownload
            // 
            btnDownload.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnDownload.Location = new Point(557, 189);
            btnDownload.Name = "btnDownload";
            btnDownload.Size = new Size(75, 23);
            btnDownload.TabIndex = 1;
            btnDownload.Text = "Download";
            btnDownload.UseVisualStyleBackColor = true;
            // 
            // txtLog
            // 
            txtLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtLog.Location = new Point(6, 22);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.Size = new Size(626, 161);
            txtLog.TabIndex = 0;
            // 
            // fbdTarget
            // 
            fbdTarget.Description = "Select a folder to save downloaded release assets";
            fbdTarget.UseDescriptionForTitle = true;
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(662, 384);
            Controls.Add(gbProgress);
            Controls.Add(gbPath);
            Controls.Add(gbUrl);
            MinimumSize = new Size(678, 423);
            Name = "FrmMain";
            Text = "GitHub Release Downloader";
            gbUrl.ResumeLayout(false);
            gbUrl.PerformLayout();
            gbPath.ResumeLayout(false);
            gbPath.PerformLayout();
            gbProgress.ResumeLayout(false);
            gbProgress.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox gbUrl;
        private TextBox txtUrl;
        private GroupBox gbPath;
        private Button btnBrowse;
        private TextBox txtPath;
        private GroupBox gbProgress;
        private Button btnDownload;
        private TextBox txtLog;
        private ProgressBar pbDownload;
        private FolderBrowserDialog fbdTarget;
    }
}
