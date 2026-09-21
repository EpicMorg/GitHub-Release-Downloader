using System.Text.RegularExpressions;

namespace GitHub_Release_Downloader
{
    /// <summary>
    ///  Owner/repository pair parsed out of whatever the user typed into the URL box.
    /// </summary>
    public sealed partial record RepoReference(string Owner, string Repo)
    {
        public override string ToString() => $"{Owner}/{Repo}";

        /// <summary>
        ///  Accepts "owner/repo", a github.com web URL, an api.github.com URL or an SSH remote.
        /// </summary>
        public static bool TryParse(string? input, out RepoReference? reference)
        {
            reference = null;

            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            var value = input.Trim();

            foreach (var pattern in Patterns)
            {
                var match = pattern.Match(value);

                if (!match.Success)
                {
                    continue;
                }

                var owner = match.Groups["owner"].Value;
                var repo = match.Groups["repo"].Value;

                if (repo.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
                {
                    repo = repo[..^4];
                }

                if (!IsSegmentValid(owner) || !IsSegmentValid(repo))
                {
                    return false;
                }

                reference = new RepoReference(owner, repo);
                return true;
            }

            return false;
        }

        private static bool IsSegmentValid(string segment) =>
            segment.Length > 0 && segment is not ("." or "..") && SegmentPattern().IsMatch(segment);

        private static readonly Regex[] Patterns =
        [
            ApiUrlPattern(),
            WebUrlPattern(),
            SshPattern(),
            ShorthandPattern(),
        ];

        [GeneratedRegex(@"^(?:https?://)?api\.github\.com/repos/(?<owner>[^/\s]+)/(?<repo>[^/\s?#]+)",
            RegexOptions.IgnoreCase)]
        private static partial Regex ApiUrlPattern();

        [GeneratedRegex(@"^(?:https?://)?(?:www\.)?github\.com/(?<owner>[^/\s]+)/(?<repo>[^/\s?#]+)",
            RegexOptions.IgnoreCase)]
        private static partial Regex WebUrlPattern();

        [GeneratedRegex(@"^git@github\.com:(?<owner>[^/\s]+)/(?<repo>[^/\s]+)$", RegexOptions.IgnoreCase)]
        private static partial Regex SshPattern();

        [GeneratedRegex(@"^(?<owner>[^/\s]+)/(?<repo>[^/\s]+)$")]
        private static partial Regex ShorthandPattern();

        [GeneratedRegex(@"^[A-Za-z0-9._-]+$")]
        private static partial Regex SegmentPattern();
    }
}
