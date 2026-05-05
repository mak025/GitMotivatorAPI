using Octokit;
using GithubMotivator.Models;
using GithubMotivator.Models.DTOs;

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
                var uri = new Uri($"search/commits?q=author:{username}", UriKind.Relative);
                // Note: Octokit doesn't have a direct SearchCommits method in some versions, 
                // but this custom call works if the API responds
                var response = await client.Connection.Get<SearchCommitsResponse>(uri, null, null);
                return response.Body.TotalCount;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<List<GithubMotivator.Models.Commit>> FetchCommitsAsync(string owner, string repo, string token, DateTime? since = null)
        {
            var client = CreateClient(token);
            var commits = new List<GithubMotivator.Models.Commit>();

            try
            {
                var request = new CommitRequest();
                if (since.HasValue)
                {
                    request.Since = since.Value;
                }

                var options = new ApiOptions { PageSize = 100, PageCount = 5 }; // Fetch up to 500 commits
                var githubCommits = await client.Repository.Commit.GetAll(owner, repo, request, options);

                foreach (var gc in githubCommits)
                {
                    commits.Add(new GithubMotivator.Models.Commit
                    {
                        Sha = gc.Sha,
                        Message = gc.Commit.Message,
                        AuthorName = gc.Commit.Author.Name,
                        AuthorEmail = gc.Commit.Author.Email,
                        Date = gc.Commit.Author.Date.DateTime
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching commits: {ex.Message}");
            }

            return commits;
        }


    }

    public class SearchCommitsResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("total_count")]
        public int TotalCount { get; set; }
    }
}
