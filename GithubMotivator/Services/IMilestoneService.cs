using GithubMotivator.Models;
namespace GithubMotivator.Services
{
    public interface IMilestoneService
    {

        public Task<Milestone> CreateMilestoneAsync(Milestone milestone);
        public Task<IEnumerable<Milestone>> GetAllMilestonesForRepoAsync(int repoId);
        public Task<Milestone> DeleteMilestoneAsync(int milestoneId);
        public Task<Milestone> GetMilestone(int milestoneId);
        public Task<Milestone> CompleteMilestone(int milestoneId);
    }
}
