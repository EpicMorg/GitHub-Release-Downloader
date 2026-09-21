using System.Text.Json.Serialization;

namespace GitHub_Release_Downloader
{
    internal sealed class GitHubRelease
    {
        [JsonPropertyName("tag_name")]
        public string? TagName { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("draft")]
        public bool Draft { get; set; }

        [JsonPropertyName("prerelease")]
        public bool PreRelease { get; set; }

        [JsonPropertyName("assets")]
        public List<GitHubAsset> Assets { get; set; } = [];

        [JsonPropertyName("zipball_url")]
        public string? ZipballUrl { get; set; }

        [JsonPropertyName("tarball_url")]
        public string? TarballUrl { get; set; }
    }

    internal sealed class GitHubAsset
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("size")]
        public long Size { get; set; }

        [JsonPropertyName("browser_download_url")]
        public string? BrowserDownloadUrl { get; set; }
    }

    /// <summary>
    ///  A single file to fetch. <see cref="ExpectedSize"/> is null when GitHub does not
    ///  advertise one (source archives), which disables the size comparison for that file.
    /// </summary>
    internal sealed record DownloadItem(string Url, string FileName, long? ExpectedSize);

    public sealed record DownloadOptions
    {
        /// <summary>
        ///  When set, files land in &lt;target&gt;/&lt;owner&gt;/&lt;repo&gt;/&lt;tag&gt;;
        ///  otherwise the owner and repository levels are dropped.
        /// </summary>
        public bool CreateRepoSubfolders { get; init; } = true;

        public bool AllReleases { get; init; }

        public bool IncludePreReleases { get; init; }

        public bool IncludeSourceArchives { get; init; }

        public bool SkipExisting { get; init; } = true;
    }

    public sealed record DownloadProgress(
        int FileIndex,
        int FileCount,
        string FileName,
        long BytesReceived,
        long? BytesTotal);

    public sealed record DownloadSummary(int Downloaded, int Skipped, int Failed);
}
