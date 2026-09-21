namespace GitHub_Release_Downloader.Cli
{
    /// <summary>
    ///  Everything the console app needs, taken from the command line only -
    ///  the CLI keeps no settings file of its own.
    /// </summary>
    internal sealed class CommandLineOptions
    {
        public RepoReference Repository { get; private init; } = null!;

        public string OutputDirectory { get; private init; } = string.Empty;

        public string? Token { get; private init; }

        public DownloadOptions Download { get; private init; } = new();

        public bool Quiet { get; private init; }

        public static bool TryParse(string[] args, out CommandLineOptions? options, out string? error)
        {
            options = null;
            error = null;

            string? repository = null;
            var output = Directory.GetCurrentDirectory();
            var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
            var latestOnly = false;
            var preReleases = false;
            var sources = false;
            var overwrite = false;
            var subFolders = true;
            var quiet = false;

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                switch (arg)
                {
                    case "-o" or "--output":
                        if (!TryTakeValue(args, ref i, arg, out output, out error))
                        {
                            return false;
                        }

                        break;

                    case "-t" or "--token":
                        if (!TryTakeValue(args, ref i, arg, out token, out error))
                        {
                            return false;
                        }

                        break;

                    case "-l" or "--latest":
                        latestOnly = true;
                        break;

                    case "-p" or "--pre":
                        preReleases = true;
                        break;

                    case "-s" or "--sources":
                        sources = true;
                        break;

                    case "--overwrite":
                        overwrite = true;
                        break;

                    case "--flat":
                        subFolders = false;
                        break;

                    case "-q" or "--quiet":
                        quiet = true;
                        break;

                    default:
                        if (arg.StartsWith('-'))
                        {
                            error = $"Unknown option '{arg}'.";
                            return false;
                        }

                        if (repository is not null)
                        {
                            error = $"Unexpected argument '{arg}' - only one repository is accepted.";
                            return false;
                        }

                        repository = arg;
                        break;
                }
            }

            if (repository is null)
            {
                error = "No repository given.";
                return false;
            }

            if (!RepoReference.TryParse(repository, out var reference) || reference is null)
            {
                error = $"'{repository}' is not a repository. Use owner/repo or a github.com URL.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(output))
            {
                error = "The output directory is empty.";
                return false;
            }

            try
            {
                output = Path.GetFullPath(output);
            }
            catch (Exception ex)
            {
                error = $"Bad output directory: {ex.Message}";
                return false;
            }

            options = new CommandLineOptions
            {
                Repository = reference,
                OutputDirectory = output,
                Token = string.IsNullOrWhiteSpace(token) ? null : token.Trim(),
                Quiet = quiet,
                Download = new DownloadOptions
                {
                    CreateRepoSubfolders = subFolders,
                    AllReleases = !latestOnly,
                    IncludePreReleases = preReleases,
                    IncludeSourceArchives = sources,
                    SkipExisting = !overwrite,
                },
            };

            return true;
        }

        private static bool TryTakeValue(
            string[] args, ref int index, string option, out string value, out string? error)
        {
            if (index + 1 >= args.Length)
            {
                value = string.Empty;
                error = $"Option '{option}' needs a value.";
                return false;
            }

            value = args[++index];
            error = null;
            return true;
        }

        public static string Usage =>
            """
            grd - download GitHub release assets

            Usage:
              grd <owner/repo | github url> [options]

            Options:
              -o, --output <dir>   Where to download to (default: current directory)
              -t, --token <pat>    GitHub token; falls back to the GITHUB_TOKEN variable
              -l, --latest         Only the latest release (default: every release)
              -p, --pre            Include pre-releases
              -s, --sources        Also grab the source archives
                  --overwrite      Always overwrite; default skips files whose size matches
                  --flat           Drop the <owner>/<repo> folders, keep only <tag>
              -q, --quiet          Only report errors and the final summary
              -h, --help           Show this help

            Layout:
              <output>/<owner>/<repo>/<tag>/<file>   (or <output>/<tag>/<file> with --flat)

            Exit codes:
              0  everything downloaded or skipped
              1  bad arguments
              2  at least one file failed
            """;
    }
}
