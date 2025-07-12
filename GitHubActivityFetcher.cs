using System.Text.Json;

namespace GitHubActivity
{
    public class GitHubActivityFetcher
    {
        private readonly HttpClient httpClient;
        private readonly JsonSerializerOptions jsonOptions;

        public GitHubActivityFetcher()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Github-Activity-CLI");

            jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<GithubEvent>> FetchUserActivityAsync(string username)
        {
            try
            {
                string url = $"https://api.github.com/users/{username}/events";
                Console.WriteLine($"Fetching activity for user; {username}...");

                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new InvalidOperationException($"User '{username}' not found.");
                }
                if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine($"User '{username}' found.");
                }
                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException($"Failed to fetch data. Status : {response.StatusCode}");
                }
                string jsonContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    throw new InvalidOperationException("No data received from GitHub API. ");
                }
                var events = JsonSerializer.Deserialize<List<GithubEvent>>(jsonContent, jsonOptions);
                return events ?? new List<GithubEvent>();
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Network error: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                throw new InvalidOperationException("Request timed out. Please check your internet connection.");
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Error parsing response: {ex.Message}");
            }
        }
    }
}