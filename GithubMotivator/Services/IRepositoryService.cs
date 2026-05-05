using GithubMotivator.Models;
using GithubMotivator.Models.DTOs;

namespace GithubMotivator.Services
{
    public interface IRepositoryService
    {
        Task<Repository> TrackRepositoryAsync(string url, string token);
        Task SyncCommitsAsync(int repositoryId, string token);
        Task<DashboardStatsDTO> GetDashboardStatsAsync(int? repositoryId = null);
        Task SyncAllRepositoriesAsync();
    }
}