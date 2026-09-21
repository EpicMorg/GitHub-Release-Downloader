namespace GitHub_Release_Downloader
{
    public partial class FrmMain : Form
    {
        private CancellationTokenSource? _cancellation;

        public FrmMain()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var settings = AppSettings.Load();

            txtUrl.Text = settings.Url;
            txtPath.Text = settings.Path;
            txtToken.Text = settings.Token;
            chkAddSubFoldersToSelectedPath.Checked = settings.AddSubFolders;
            chkLatestOnly.Checked = settings.LatestOnly;
            chkPreRelease.Checked = settings.PreReleases;
            chkSources.Checked = settings.Sources;
            rbOverwrite.Checked = settings.Overwrite;
            rbSkip.Checked = !settings.Overwrite;
        }

        private void SaveSettings()
        {
            var settings = new AppSettings
            {
                Url = txtUrl.Text.Trim(),
                Path = txtPath.Text.Trim(),
                Token = txtToken.Text.Trim(),
                AddSubFolders = chkAddSubFoldersToSelectedPath.Checked,
                LatestOnly = chkLatestOnly.Checked,
                PreReleases = chkPreRelease.Checked,
                Sources = chkSources.Checked,
                Overwrite = rbOverwrite.Checked,
            };

            if (!settings.TrySave(out var error))
            {
                Log($"Could not save settings: {error}");
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var current = txtPath.Text.Trim();

            if (current.Length > 0 && Directory.Exists(current))
            {
                fbdTarget.SelectedPath = current;
            }

            if (fbdTarget.ShowDialog(this) == DialogResult.OK)
            {
                txtPath.Text = fbdTarget.SelectedPath;
            }
        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            if (_cancellation is not null)
            {
                _cancellation.Cancel();
                return;
            }

            if (!RepoReference.TryParse(txtUrl.Text, out var reference) || reference is null)
            {
                Warn("Enter a repository as owner/repo or as a github.com URL.", txtUrl);
                return;
            }

            var target = txtPath.Text.Trim();

            if (target.Length == 0)
            {
                Warn("Choose the folder to download into.", txtPath);
                return;
            }

            try
            {
                target = Path.GetFullPath(target);
                Directory.CreateDirectory(target);
            }
            catch (Exception ex)
            {
                Warn($"That path is not usable: {ex.Message}", txtPath);
                return;
            }

            var options = new DownloadOptions
            {
                CreateRepoSubfolders = chkAddSubFoldersToSelectedPath.Checked,
                AllReleases = !chkLatestOnly.Checked,
                IncludePreReleases = chkPreRelease.Checked,
                IncludeSourceArchives = chkSources.Checked,
                SkipExisting = rbSkip.Checked,
            };

            // Persist now as well, so a crash mid-download does not lose the setup.
            SaveSettings();

            _cancellation = new CancellationTokenSource();
            SetRunning(true);

            txtLog.Clear();
            Log($"{reference} -> {target}");

            try
            {
                using var downloader = new ReleaseDownloader(Log, txtToken.Text);
                var progress = new Progress<DownloadProgress>(Report);

                var summary = await downloader.RunAsync(
                    reference, target, options, progress, _cancellation.Token);

                Log($"Done: {summary.Downloaded} downloaded, {summary.Skipped} skipped, " +
                    $"{summary.Failed} failed.");
            }
            catch (OperationCanceledException)
            {
                Log("Cancelled.");
            }
            catch (Exception ex)
            {
                Log($"ERROR: {ex.Message}");
            }
            finally
            {
                _cancellation.Dispose();
                _cancellation = null;
                SetRunning(false);
                pbDownload.Value = 0;
            }
        }

        private void Report(DownloadProgress progress)
        {
            var completed = progress.FileIndex - 1;
            var fraction = progress.BytesTotal is > 0
                ? Math.Min(1d, (double)progress.BytesReceived / progress.BytesTotal.Value)
                : 0d;

            var percent = (int)Math.Round((completed + fraction) / progress.FileCount * 100);
            percent = Math.Clamp(percent, 0, 100);

            if (pbDownload.Value != percent)
            {
                pbDownload.Value = percent;
            }
        }

        private void Log(string message)
        {
            // The downloader reports from a thread pool thread, so stamp the time here
            // and hand the finished line to the UI thread.
            var line = $"{DateTime.Now:HH:mm:ss}  {message}{Environment.NewLine}";

            if (!txtLog.IsHandleCreated || txtLog.IsDisposed)
            {
                return;
            }

            if (txtLog.InvokeRequired)
            {
                txtLog.BeginInvoke(() => txtLog.AppendText(line));
                return;
            }

            txtLog.AppendText(line);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Stop an in-flight download so its callbacks do not outlive the handle.
            _cancellation?.Cancel();
            SaveSettings();
            base.OnFormClosing(e);
        }

        private void Warn(string message, Control focus)
        {
            MessageBox.Show(this, message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            focus.Focus();
        }

        private void SetRunning(bool running)
        {
            txtUrl.Enabled = !running;
            txtToken.Enabled = !running;
            txtPath.Enabled = !running;
            btnBrowse.Enabled = !running;
            chkSources.Enabled = !running;
            chkAddSubFoldersToSelectedPath.Enabled = !running;
            chkPreRelease.Enabled = !running;
            chkLatestOnly.Enabled = !running;
            rbSkip.Enabled = !running;
            rbOverwrite.Enabled = !running;

            btnDownload.Text = running ? "Cancel" : "Download";
            UseWaitCursor = running;
        }
    }
}
