using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace GitHub_Release_Downloader
{
    /// <summary>
    ///  Pulls release assets from the GitHub API into
    ///  &lt;target&gt;/&lt;owner&gt;/&lt;repo&gt;/&lt;tag&gt;/&lt;file&gt;.
    /// </summary>
    internal sealed class ReleaseDownloader : IDisposable
    {
        private const string ApiRoot = "https://api.github.com";
        private const int PageSize = 100;

        private readonly HttpClient _client;
        private readonly Action<string> _log;
        private readonly bool _authenticated;

        public ReleaseDownloader(Action<string> log, string? token = null)
        {
            _log = log;
            _authenticated = !string.IsNullOrWhiteSpace(token);

            _client = new HttpClient
            {
                Timeout = Timeout.InfiniteTimeSpan,
            };

            // GitHub rejects requests without a User-Agent.
            _client.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue("GitHub-Release-Downloader", "1.0"));
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
            _client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");

            if (_authenticated)
            {
                // HttpClient drops this header on a cross-origin redirect, so the token
                // never reaches the CDN host that actually serves the asset bytes.
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token!.Trim());
            }
        }

        public void Dispose() => _client.Dispose();

        public async Task<DownloadSummary> RunAsync(
            RepoReference reference,
            string targetRoot,
            DownloadOptions options,
            IProgress<DownloadProgress>? progress,
            CancellationToken cancellationToken)
        {
            var releases = await GetReleasesAsync(reference, options, cancellationToken)
                .ConfigureAwait(false);

            if (releases.Count == 0)
            {
                _log("No matching releases found.");
                return new DownloadSummary(0, 0, 0);
            }

            var plan = new List<(string Directory, DownloadItem Item)>();

            foreach (var release in releases)
            {
                var tag = release.TagName;

                if (string.IsNullOrWhiteSpace(tag))
                {
                    _log($"Skipping a release without a tag ({release.Name ?? "unnamed"}).");
                    continue;
                }

                var directory = Path.Combine(
                    targetRoot,
                    SanitizeSegment(reference.Owner),
                    SanitizeSegment(reference.Repo),
                    SanitizeSegment(tag));

                foreach (var item in EnumerateItems(reference, release, tag, options))
                {
                    plan.Add((directory, item));
                }
            }

            if (plan.Count == 0)
            {
                _log("Nothing to download: the selected releases carry no files.");
                return new DownloadSummary(0, 0, 0);
            }

            _log($"{releases.Count} release(s), {plan.Count} file(s) queued.");

            var downloaded = 0;
            var skipped = 0;
            var failed = 0;

            for (var i = 0; i < plan.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var (directory, item) = plan[i];
                var path = Path.Combine(directory, item.FileName);

                if (ShouldSkip(path, item, options))
                {
                    skipped++;
                    progress?.Report(new DownloadProgress(
                        i + 1, plan.Count, item.FileName, item.ExpectedSize ?? 0, item.ExpectedSize));
                    continue;
                }

                try
                {
                    Directory.CreateDirectory(directory);
                    await DownloadFileAsync(item, path, i, plan.Count, progress, cancellationToken)
                        .ConfigureAwait(false);
                    downloaded++;
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    failed++;
                    _log($"FAILED {item.FileName}: {ex.Message}");
                }
            }

            return new DownloadSummary(downloaded, skipped, failed);
        }

        private async Task<List<GitHubRelease>> GetReleasesAsync(
            RepoReference reference,
            DownloadOptions options,
            CancellationToken cancellationToken)
        {
            if (!options.AllReleases)
            {
                var latest = await GetLatestAsync(reference, options, cancellationToken)
                    .ConfigureAwait(false);

                return latest is null ? [] : [latest];
            }

            var all = new List<GitHubRelease>();

            for (var page = 1; ; page++)
            {
                var url = $"{ApiRoot}/repos/{reference.Owner}/{reference.Repo}" +
                          $"/releases?per_page={PageSize}&page={page}";

                var batch = await GetJsonAsync<List<GitHubRelease>>(url, cancellationToken)
                    .ConfigureAwait(false);

                if (batch is null || batch.Count == 0)
                {
                    break;
                }

                all.AddRange(batch);

                if (batch.Count < PageSize)
                {
                    break;
                }
            }

            return [.. all.Where(r => !r.Draft && (options.IncludePreReleases || !r.PreRelease))];
        }

        private async Task<GitHubRelease?> GetLatestAsync(
            RepoReference reference,
            DownloadOptions options,
            CancellationToken cancellationToken)
        {
            // /releases/latest never returns a pre-release, so when the user wants those
            // included the newest entry of the full list is the right answer instead.
            if (options.IncludePreReleases)
            {
                var url = $"{ApiRoot}/repos/{reference.Owner}/{reference.Repo}/releases?per_page=1";
                var batch = await GetJsonAsync<List<GitHubRelease>>(url, cancellationToken)
                    .ConfigureAwait(false);

                return batch?.FirstOrDefault(r => !r.Draft);
            }

            return await GetJsonAsync<GitHubRelease>(
                $"{ApiRoot}/repos/{reference.Owner}/{reference.Repo}/releases/latest",
                cancellationToken).ConfigureAwait(false);
        }

        private async Task<T?> GetJsonAsync<T>(string url, CancellationToken cancellationToken)
        {
            using var response = await _client.GetAsync(url, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(DescribeFailure(response, url));
            }

            return await response.Content
                .ReadFromJsonAsync<T>(cancellationToken)
                .ConfigureAwait(false);
        }

        private string DescribeFailure(HttpResponseMessage response, string url)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return _authenticated
                    ? "Repository or release not found (does the token grant access to it?)."
                    : "Repository or release not found - private repositories need a token.";
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return "GitHub rejected the token (401). Check it on the Settings tab.";
            }

            var remaining = response.Headers.TryGetValues("X-RateLimit-Remaining", out var values)
                ? values.FirstOrDefault()
                : null;

            if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.TooManyRequests &&
                remaining == "0")
            {
                return _authenticated
                    ? "GitHub API rate limit exhausted (5000 requests/hour)."
                    : "GitHub API rate limit exhausted - add a token on the Settings tab for 5000/hour.";
            }

            return $"{(int)response.StatusCode} {response.ReasonPhrase} for {url}";
        }

        private static IEnumerable<DownloadItem> EnumerateItems(
            RepoReference reference,
            GitHubRelease release,
            string tag,
            DownloadOptions options)
        {
            foreach (var asset in release.Assets)
            {
                if (string.IsNullOrWhiteSpace(asset.BrowserDownloadUrl) ||
                    string.IsNullOrWhiteSpace(asset.Name))
                {
                    continue;
                }

                yield return new DownloadItem(
                    asset.BrowserDownloadUrl,
                    SanitizeSegment(asset.Name),
                    asset.Size > 0 ? asset.Size : null);
            }

            if (!options.IncludeSourceArchives)
            {
                yield break;
            }

            var stem = SanitizeSegment($"{reference.Repo}-{tag}-source");

            if (!string.IsNullOrWhiteSpace(release.ZipballUrl))
            {
                yield return new DownloadItem(release.ZipballUrl, $"{stem}.zip", null);
            }

            if (!string.IsNullOrWhiteSpace(release.TarballUrl))
            {
                yield return new DownloadItem(release.TarballUrl, $"{stem}.tar.gz", null);
            }
        }

        private bool ShouldSkip(string path, DownloadItem item, DownloadOptions options)
        {
            if (!File.Exists(path) || !options.SkipExisting)
            {
                return false;
            }

            var name = item.FileName;

            if (item.ExpectedSize is null)
            {
                // No advertised size to compare against - trust the file on disk.
                _log($"SKIP {name} (already present, size unknown upstream)");
                return true;
            }

            var actual = new FileInfo(path).Length;

            if (actual == item.ExpectedSize)
            {
                _log($"SKIP {name} (already present, {actual:N0} bytes)");
                return true;
            }

            _log($"REDOWNLOAD {name} (size mismatch: {actual:N0} on disk, {item.ExpectedSize:N0} upstream)");
            return false;
        }

        private async Task DownloadFileAsync(
            DownloadItem item,
            string path,
            int index,
            int total,
            IProgress<DownloadProgress>? progress,
            CancellationToken cancellationToken)
        {
            _log($"GET {item.FileName}");

            using var response = await _client
                .GetAsync(item.Url, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(DescribeFailure(response, item.Url));
            }

            var expected = response.Content.Headers.ContentLength ?? item.ExpectedSize;
            var temporary = path + ".part";

            await using (var source = await response.Content
                             .ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
            await using (var destination = new FileStream(
                             temporary, FileMode.Create, FileAccess.Write, FileShare.None,
                             bufferSize: 81920, useAsync: true))
            {
                var buffer = new byte[81920];
                long received = 0;
                int read;

                while ((read = await source.ReadAsync(buffer, cancellationToken)
                           .ConfigureAwait(false)) > 0)
                {
                    await destination.WriteAsync(buffer.AsMemory(0, read), cancellationToken)
                        .ConfigureAwait(false);

                    received += read;
                    progress?.Report(new DownloadProgress(
                        index + 1, total, item.FileName, received, expected));
                }
            }

            File.Move(temporary, path, overwrite: true);
            _log($"OK   {item.FileName}");
        }

        private static string SanitizeSegment(string value)
        {
            var cleaned = string.Join("_", value.Split(Path.GetInvalidFileNameChars()));
            return cleaned.Trim().TrimEnd('.') is { Length: > 0 } result ? result : "_";
        }
    }
}
