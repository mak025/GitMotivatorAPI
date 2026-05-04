using GithubMotivator.Models;

namespace GithubMotivator.Services
{
    public interface IGithubService
    {
        Task<int> GetUserCommitCountAsync(string username, string token);
        Task<int> GetUserPullRequestCountAsync(string username, string token);
        // Add more methods as needed for motivator stats
    }
}
