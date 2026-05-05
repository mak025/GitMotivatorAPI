using GithubMotivator.Models;
using GithubMotivator.Models.DTOs;

namespace GithubMotivator.Services
{
    public interface IGithubService
    {
        Task<int> GetUserCommitCountAsync(string username, string token);
        Task<List<GithubMotivator.Models.Commit>> FetchCommitsAsync(string owner, string repo, string token, DateTime? since = null);
    }
}