namespace GitHub_Release_Downloader.Cli
{
    internal static class Program
    {
        private static async Task<int> Main(string[] args)
        {
            if (args.Length == 0 || args.Contains("-h") || args.Contains("--help"))
            {
                Console.WriteLine(CommandLineOptions.Usage);
                return args.Length == 0 ? 1 : 0;
            }

            if (!CommandLineOptions.TryParse(args, out var options, out var error) || options is null)
            {
                Console.Error.WriteLine($"grd: {error}");
                Console.Error.WriteLine("Try 'grd --help'.");
                return 1;
            }

            using var cancellation = new CancellationTokenSource();

            Console.CancelKeyPress += (_, e) =>
            {
                // Let the current write finish and unwind cleanly instead of being killed.
                e.Cancel = true;
                cancellation.Cancel();
                Console.Error.WriteLine("Cancelling...");
            };

            Action<string> log = options.Quiet ? Quiet : Verbose;

            try
            {
                Directory.CreateDirectory(options.OutputDirectory);

                using var downloader = new ReleaseDownloader(log, options.Token);

                var summary = await downloader.RunAsync(
                    options.Repository,
                    options.OutputDirectory,
                    options.Download,
                    null,
                    cancellation.Token);

                Console.WriteLine(
                    $"{summary.Downloaded} downloaded, {summary.Skipped} skipped, {summary.Failed} failed.");

                return summary.Failed > 0 ? 2 : 0;
            }
            catch (OperationCanceledException)
            {
                Console.Error.WriteLine("Cancelled.");
                return 2;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"grd: {ex.Message}");
                return 2;
            }
        }

        private static void Verbose(string message) => Console.WriteLine(message);

        private static void Quiet(string message)
        {
            if (message.StartsWith("FAILED", StringComparison.Ordinal))
            {
                Console.Error.WriteLine(message);
            }
        }
    }
}
