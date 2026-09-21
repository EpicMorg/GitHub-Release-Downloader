using System.Text.Json.Serialization;

namespace GitHub_Release_Downloader
{
    /// <summary>
    ///  Source-generated metadata for the GitHub API payloads. Reflection-based
    ///  serialization does not survive trimming or native AOT, so every type the
    ///  downloader reads has to be declared here.
    /// </summary>
    [JsonSerializable(typeof(GitHubRelease))]
    [JsonSerializable(typeof(List<GitHubRelease>))]
    internal sealed partial class GitHubJsonContext : JsonSerializerContext
    {
    }
}
