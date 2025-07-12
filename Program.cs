
namespace GitHubActivity
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                var fetcher = new GitHubActivityFetcher();
                var activities = await fetcher.FetchUserActivityAsync("ysaricam");
                return;
            }

        }
    }
}