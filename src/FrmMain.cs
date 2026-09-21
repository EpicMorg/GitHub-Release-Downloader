namespace GitHub_Release_Downloader
{
    public partial class FrmMain : Form
    {
        private CancellationTokenSource? _cancellation;

        public FrmMain()
        {
            InitializeComponent();
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
                // "latest only" means the newest stable release; otherwise take the lot,
                // pre-releases included, since there is no separate switch for them.
                AllReleases = !chkLatestOnly.Checked,
                IncludePreReleases = !chkLatestOnly.Checked,
                IncludeSourceArchives = chkSources.Checked,
                SkipExisting = rbSkip.Checked,
            };

            _cancellation = new CancellationTokenSource();
            SetRunning(true);

            txtLog.Clear();
            Log($"{reference} -> {target}");

            try
            {
                using var downloader = new ReleaseDownloader(Log);
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
            txtLog.AppendText($"{DateTime.Now:HH:mm:ss}  {message}{Environment.NewLine}");
        }

        private void Warn(string message, Control focus)
        {
            MessageBox.Show(this, message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            focus.Focus();
        }

        private void SetRunning(bool running)
        {
            txtUrl.Enabled = !running;
            txtPath.Enabled = !running;
            btnBrowse.Enabled = !running;
            chkSources.Enabled = !running;
            chkLatestOnly.Enabled = !running;
            rbSkip.Enabled = !running;
            rbOverwrite.Enabled = !running;

            btnDownload.Text = running ? "Cancel" : "Download";
            UseWaitCursor = running;
        }
    }
}
