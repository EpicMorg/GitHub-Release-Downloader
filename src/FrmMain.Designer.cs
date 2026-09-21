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
            tabsMain = new TabControl();
            tabMain = new TabPage();
            gbUrl = new GroupBox();
            txtUrl = new TextBox();
            gbPath = new GroupBox();
            btnBrowse = new Button();
            txtPath = new TextBox();
            gbProgress = new GroupBox();
            pbDownload = new ProgressBar();
            btnDownload = new Button();
            txtLog = new TextBox();
            tabSettings = new TabPage();
            gbToken = new GroupBox();
            lblTokenHint = new Label();
            txtToken = new TextBox();
            gbOptions = new GroupBox();
            chkLatestOnly = new CheckBox();
            chkPreRelease = new CheckBox();
            chkSources = new CheckBox();
            gbExisting = new GroupBox();
            rbSkip = new RadioButton();
            rbOverwrite = new RadioButton();
            fbdTarget = new FolderBrowserDialog();
            tabsMain.SuspendLayout();
            tabMain.SuspendLayout();
            gbUrl.SuspendLayout();
            gbPath.SuspendLayout();
            gbProgress.SuspendLayout();
            tabSettings.SuspendLayout();
            gbToken.SuspendLayout();
            gbOptions.SuspendLayout();
            gbExisting.SuspendLayout();
            SuspendLayout();
            // 
            // tabsMain
            // 
            tabsMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabsMain.Controls.Add(tabMain);
            tabsMain.Controls.Add(tabSettings);
            tabsMain.Location = new Point(12, 12);
            tabsMain.Name = "tabsMain";
            tabsMain.SelectedIndex = 0;
            tabsMain.Size = new Size(638, 360);
            tabsMain.TabIndex = 0;
            // 
            // tabMain
            // 
            tabMain.Controls.Add(gbUrl);
            tabMain.Controls.Add(gbPath);
            tabMain.Controls.Add(gbProgress);
            tabMain.Location = new Point(4, 24);
            tabMain.Name = "tabMain";
            tabMain.Padding = new Padding(3);
            tabMain.Size = new Size(630, 332);
            tabMain.TabIndex = 0;
            tabMain.Text = "Main";
            tabMain.UseVisualStyleBackColor = true;
            // 
            // gbUrl
            // 
            gbUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbUrl.Controls.Add(txtUrl);
            gbUrl.Location = new Point(6, 6);
            gbUrl.Name = "gbUrl";
            gbUrl.Size = new Size(618, 65);
            gbUrl.TabIndex = 0;
            gbUrl.TabStop = false;
            gbUrl.Text = "URL";
            // 
            // txtUrl
            // 
            txtUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtUrl.Location = new Point(6, 22);
            txtUrl.Name = "txtUrl";
            txtUrl.PlaceholderText = "owner/repo or https://github.com/owner/repo";
            txtUrl.Size = new Size(606, 23);
            txtUrl.TabIndex = 0;
            // 
            // gbPath
            // 
            gbPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbPath.Controls.Add(btnBrowse);
            gbPath.Controls.Add(txtPath);
            gbPath.Location = new Point(6, 77);
            gbPath.Name = "gbPath";
            gbPath.Size = new Size(618, 65);
            gbPath.TabIndex = 1;
            gbPath.TabStop = false;
            gbPath.Text = "Path";
            // 
            // btnBrowse
            // 
            btnBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowse.Location = new Point(537, 22);
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
            txtPath.Size = new Size(525, 23);
            txtPath.TabIndex = 0;
            // 
            // gbProgress
            // 
            gbProgress.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gbProgress.Controls.Add(pbDownload);
            gbProgress.Controls.Add(btnDownload);
            gbProgress.Controls.Add(txtLog);
            gbProgress.Location = new Point(6, 148);
            gbProgress.Name = "gbProgress";
            gbProgress.Size = new Size(618, 178);
            gbProgress.TabIndex = 2;
            gbProgress.TabStop = false;
            gbProgress.Text = "Progress";
            // 
            // pbDownload
            // 
            pbDownload.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pbDownload.Location = new Point(6, 146);
            pbDownload.Name = "pbDownload";
            pbDownload.Size = new Size(525, 23);
            pbDownload.TabIndex = 1;
            // 
            // btnDownload
            // 
            btnDownload.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnDownload.Location = new Point(537, 146);
            btnDownload.Name = "btnDownload";
            btnDownload.Size = new Size(75, 23);
            btnDownload.TabIndex = 2;
            btnDownload.Text = "Download";
            btnDownload.UseVisualStyleBackColor = true;
            btnDownload.Click += btnDownload_Click;
            // 
            // txtLog
            // 
            txtLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtLog.Location = new Point(6, 22);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(606, 116);
            txtLog.TabIndex = 0;
            // 
            // tabSettings
            // 
            tabSettings.Controls.Add(gbToken);
            tabSettings.Controls.Add(gbOptions);
            tabSettings.Controls.Add(gbExisting);
            tabSettings.Location = new Point(4, 24);
            tabSettings.Name = "tabSettings";
            tabSettings.Padding = new Padding(3);
            tabSettings.Size = new Size(630, 332);
            tabSettings.TabIndex = 1;
            tabSettings.Text = "Settings";
            tabSettings.UseVisualStyleBackColor = true;
            // 
            // gbToken
            // 
            gbToken.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbToken.Controls.Add(lblTokenHint);
            gbToken.Controls.Add(txtToken);
            gbToken.Location = new Point(6, 6);
            gbToken.Name = "gbToken";
            gbToken.Size = new Size(618, 82);
            gbToken.TabIndex = 0;
            gbToken.TabStop = false;
            gbToken.Text = "Personal access token";
            // 
            // lblTokenHint
            // 
            lblTokenHint.AutoSize = true;
            lblTokenHint.Location = new Point(6, 52);
            lblTokenHint.Name = "lblTokenHint";
            lblTokenHint.Size = new Size(605, 15);
            lblTokenHint.TabIndex = 1;
            lblTokenHint.Text = "Optional. Lifts the API limit from 60 to 5000 requests per hour and unlocks private repositories. Not stored on disk.";
            // 
            // txtToken
            // 
            txtToken.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtToken.Location = new Point(6, 22);
            txtToken.Name = "txtToken";
            txtToken.PlaceholderText = "ghp_... (optional)";
            txtToken.Size = new Size(606, 23);
            txtToken.TabIndex = 0;
            txtToken.UseSystemPasswordChar = true;
            // 
            // gbOptions
            // 
            gbOptions.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbOptions.Controls.Add(chkPreRelease);
            gbOptions.Controls.Add(chkLatestOnly);
            gbOptions.Controls.Add(chkSources);
            gbOptions.Location = new Point(6, 94);
            gbOptions.Name = "gbOptions";
            gbOptions.Size = new Size(618, 107);
            gbOptions.TabIndex = 1;
            gbOptions.TabStop = false;
            gbOptions.Text = "What to download";
            // 
            // chkLatestOnly
            // 
            chkLatestOnly.AutoSize = true;
            chkLatestOnly.Location = new Point(6, 22);
            chkLatestOnly.Name = "chkLatestOnly";
            chkLatestOnly.Size = new Size(176, 19);
            chkLatestOnly.TabIndex = 0;
            chkLatestOnly.Text = "Download latest release only";
            chkLatestOnly.UseVisualStyleBackColor = true;
            // 
            // chkPreRelease
            // 
            chkPreRelease.AutoSize = true;
            chkPreRelease.Location = new Point(6, 47);
            chkPreRelease.Name = "chkPreRelease";
            chkPreRelease.Size = new Size(141, 19);
            chkPreRelease.TabIndex = 1;
            chkPreRelease.Text = "Download prereleases";
            // 
            // chkSources
            // 
            chkSources.AutoSize = true;
            chkSources.Location = new Point(6, 72);
            chkSources.Name = "chkSources";
            chkSources.Size = new Size(123, 19);
            chkSources.TabIndex = 2;
            chkSources.Text = "Download sources";
            chkSources.UseVisualStyleBackColor = true;
            // 
            // gbExisting
            // 
            gbExisting.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbExisting.Controls.Add(rbSkip);
            gbExisting.Controls.Add(rbOverwrite);
            gbExisting.Location = new Point(6, 207);
            gbExisting.Name = "gbExisting";
            gbExisting.Size = new Size(618, 82);
            gbExisting.TabIndex = 2;
            gbExisting.TabStop = false;
            gbExisting.Text = "Files already on disk";
            // 
            // rbSkip
            // 
            rbSkip.AutoSize = true;
            rbSkip.Checked = true;
            rbSkip.Location = new Point(6, 22);
            rbSkip.Name = "rbSkip";
            rbSkip.Size = new Size(252, 19);
            rbSkip.TabIndex = 0;
            rbSkip.TabStop = true;
            rbSkip.Text = "Skip, but re-download when the size differs";
            rbSkip.UseVisualStyleBackColor = true;
            // 
            // rbOverwrite
            // 
            rbOverwrite.AutoSize = true;
            rbOverwrite.Location = new Point(6, 47);
            rbOverwrite.Name = "rbOverwrite";
            rbOverwrite.Size = new Size(114, 19);
            rbOverwrite.TabIndex = 1;
            rbOverwrite.Text = "Always overwrite";
            rbOverwrite.UseVisualStyleBackColor = true;
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
            Controls.Add(tabsMain);
            MinimumSize = new Size(678, 423);
            Name = "FrmMain";
            Text = "GitHub Release Downloader";
            tabsMain.ResumeLayout(false);
            tabMain.ResumeLayout(false);
            gbUrl.ResumeLayout(false);
            gbUrl.PerformLayout();
            gbPath.ResumeLayout(false);
            gbPath.PerformLayout();
            gbProgress.ResumeLayout(false);
            gbProgress.PerformLayout();
            tabSettings.ResumeLayout(false);
            gbToken.ResumeLayout(false);
            gbToken.PerformLayout();
            gbOptions.ResumeLayout(false);
            gbOptions.PerformLayout();
            gbExisting.ResumeLayout(false);
            gbExisting.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabsMain;
        private TabPage tabMain;
        private TabPage tabSettings;
        private GroupBox gbUrl;
        private TextBox txtUrl;
        private GroupBox gbPath;
        private Button btnBrowse;
        private TextBox txtPath;
        private GroupBox gbProgress;
        private Button btnDownload;
        private TextBox txtLog;
        private ProgressBar pbDownload;
        private GroupBox gbToken;
        private TextBox txtToken;
        private Label lblTokenHint;
        private GroupBox gbOptions;
        private CheckBox chkSources;
        private CheckBox chkPreRelease;
        private CheckBox chkLatestOnly;
        private GroupBox gbExisting;
        private RadioButton rbSkip;
        private RadioButton rbOverwrite;
        private FolderBrowserDialog fbdTarget;
    }
}
