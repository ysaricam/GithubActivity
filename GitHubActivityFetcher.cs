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
            httpClient.DefaultRequestHeaders.Add("User-Agent", "GitHub-Activity-CLI");
            
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
                
                Console.WriteLine($"Fetching activity for user: {username}...");
                
                HttpResponseMessage response = await httpClient.GetAsync(url);
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new InvalidOperationException($"User '{username}' not found.");
                }
                
                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException($"Failed to fetch data. Status: {response.StatusCode}");
                }

                string jsonContent = await response.Content.ReadAsStringAsync();
                
                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    throw new InvalidOperationException("No data received from GitHub API.");
                }

                var events = JsonSerializer.Deserialize<List<GithubEvent>>(jsonContent, jsonOptions);
                return events ?? new List<GithubEvent>();
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Network error: {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                throw new InvalidOperationException("Request timed out. Please check your internet connection.");
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Error parsing response: {ex.Message}");
            }
        }

        public void DisplayActivity(List<GithubEvent> events, string username)
        {
            if (events == null || !events.Any())
            {
                Console.WriteLine($"No recent activity found for user '{username}'.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine($"Recent activity for {username}:");
            Console.WriteLine(new string('-', 50));

            foreach (var eventItem in events.Take(20)) // Son 20 etkinliği göster
            {
                string activityDescription = FormatActivityDescription(eventItem);
                if (!string.IsNullOrEmpty(activityDescription))
                {
                    Console.WriteLine($"- {activityDescription}");
                }
            }

            Console.WriteLine();
            Console.WriteLine($"Showing {Math.Min(events.Count, 20)} most recent activities.");
        }

        private string FormatActivityDescription(GithubEvent eventItem)
        {
            string repoName = eventItem.repo?.name ?? "unknown repository";
            
            switch (eventItem.type)
            {
                case "PushEvent":
                    int commitCount = eventItem.payload?.commits?.Count ?? 0;
                    string commitText = commitCount == 1 ? "commit" : "commits";
                    return $"Pushed {commitCount} {commitText} to {repoName}";

                case "CreateEvent":
                    if (eventItem.payload?.ref_type == "repository")
                    {
                        return $"Created repository {repoName}";
                    }
                    else if (eventItem.payload?.ref_type == "branch")
                    {
                        return $"Created branch '{eventItem.payload.@ref}' in {repoName}";
                    }
                    else if (eventItem.payload?.ref_type == "tag")
                    {
                        return $"Created tag '{eventItem.payload.@ref}' in {repoName}";
                    }
                    return $"Created {eventItem.payload?.ref_type} in {repoName}";

                case "DeleteEvent":
                    return $"Deleted {eventItem.payload?.ref_type} '{eventItem.payload?.@ref}' in {repoName}";

                case "IssuesEvent":
                    string issueAction = eventItem.payload?.action ?? "unknown action";
                    int issueNumber = eventItem.payload?.issue?.number ?? 0;
                    return $"{char.ToUpper(issueAction[0]) + issueAction.Substring(1)} issue #{issueNumber} in {repoName}";

                case "PullRequestEvent":
                    string prAction = eventItem.payload?.action ?? "unknown action";
                    int prNumber = eventItem.payload?.pull_request?.number ?? 0;
                    return $"{char.ToUpper(prAction[0]) + prAction.Substring(1)} pull request #{prNumber} in {repoName}";

                case "WatchEvent":
                    return $"Starred {repoName}";

                case "ForkEvent":
                    return $"Forked {repoName}";

                case "ReleaseEvent":
                    string releaseAction = eventItem.payload?.action ?? "published";
                    return $"{char.ToUpper(releaseAction[0]) + releaseAction.Substring(1)} release in {repoName}";

                case "PublicEvent":
                    return $"Made {repoName} public";

                case "MemberEvent":
                    string memberAction = eventItem.payload?.action ?? "added";
                    return $"{char.ToUpper(memberAction[0]) + memberAction.Substring(1)} member to {repoName}";

                case "IssueCommentEvent":
                    string commentAction = eventItem.payload?.action ?? "created";
                    return $"{char.ToUpper(commentAction[0]) + commentAction.Substring(1)} comment on issue in {repoName}";

                case "PullRequestReviewEvent":
                    string reviewAction = eventItem.payload?.action ?? "submitted";
                    return $"{char.ToUpper(reviewAction[0]) + reviewAction.Substring(1)} pull request review in {repoName}";

                case "PullRequestReviewCommentEvent":
                    string reviewCommentAction = eventItem.payload?.action ?? "created";
                    return $"{char.ToUpper(reviewCommentAction[0]) + reviewCommentAction.Substring(1)} review comment in {repoName}";

                case "GollumEvent":
                    return $"Updated wiki in {repoName}";

                default:
                    return $"Performed {eventItem.type.Replace("Event", "").ToLower()} in {repoName}";
            }
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }   
}