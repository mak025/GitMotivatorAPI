using Octokit;
using GithubMotivator.Models;

namespace GithubMotivator.Services
{
    public class GithubService : IGithubService
    {
        private GitHubClient CreateClient(string token)
        {
            var client = new GitHubClient(new ProductHeaderValue("GithubMotivator"))
            {
                Credentials = new Credentials(token)
            };
            return client;
        }

        public async Task<int> GetUserCommitCountAsync(string username, string token)
        {
            var client = CreateClient(token);
            try
            {
                // Search for commits by author
                // If Octokit doesn't have a specific SearchCommits, we use the connection directly
                var uri = new Uri($"search/commits?q=author:{username}", UriKind.Relative);
                var response = await client.Connection.Get<SearchCommitsResponse>(uri, null, null);
                return response.Body.TotalCount;
            }
            catch (Exception ex)
            {
                // Log error or handle appropriately
                Console.WriteLine($"Error fetching commits for {username}: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> GetUserPullRequestCountAsync(string username, string token)
        {
            var client = CreateClient(token);
            try
            {
                // Search for PRs by author
                var request = new SearchIssuesRequest
                {
                    Author = username,
                    Type = IssueTypeQualifier.PullRequest
                };
                var result = await client.Search.SearchIssues(request);
                return result.TotalCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching PRs for {username}: {ex.Message}");
                return 0;
            }
        }
    }

    public class SearchCommitsResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("total_count")]
        public int TotalCount { get; set; }
    }
}
