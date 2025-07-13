
namespace GitHubActivity
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                ShowHelp();
                return;
            }

            string username = args[0];

            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Error: Please provide a valid GitHub username.");
                ShowHelp();
                return;
            }

            if (args.Length > 1 && (args[1] == "--help" || args[1] == "-h"))
            {
                ShowHelp();
                return;
            }

            var fetcher = new GitHubActivityFetcher();

            try
            {
                var activities = await fetcher.FetchUserActivityAsync(username);
                fetcher.DisplayActivity(activities, username);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
            finally
            {
                fetcher.Dispose();
            }
        
            static void ShowHelp()
            {
                Console.WriteLine("GitHub Activity CLI");
                Console.WriteLine("==================");
                Console.WriteLine();
                Console.WriteLine("Fetches and displays recent GitHub activity for a specified user.");
                Console.WriteLine();
                Console.WriteLine("Usage:");
                Console.WriteLine("  github-activity <username>");
                Console.WriteLine("  github-activity <username> [--help|-h]");
                Console.WriteLine();
                Console.WriteLine("Arguments:");
                Console.WriteLine("  <username>    GitHub username to fetch activity for");
                Console.WriteLine();
                Console.WriteLine("Options:");
                Console.WriteLine("  --help, -h    Show this help message");
                Console.WriteLine();
                Console.WriteLine("Examples:");
                Console.WriteLine("  github-activity kamranahmedse");
                Console.WriteLine("  github-activity octocat");
                Console.WriteLine("  github-activity torvalds");
                Console.WriteLine();
                Console.WriteLine("Note: This tool displays the most recent 20 activities for the specified user.");
            }
        }
    }
}