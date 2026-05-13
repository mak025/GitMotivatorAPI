using GithubMotivator.Data;
using GithubMotivator.Models;

namespace GithubMotivator.Services
{
    public class MilestoneService : IMilestoneService
    {
        private readonly AppDbContext _context;

        public MilestoneService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Milestone> CreateMilestoneAsync(Milestone milestone)
        {

            _context.Milestones.Add(milestone);
            await _context.SaveChangesAsync();
            return milestone;
        }

        public async Task<IEnumerable<Milestone>> GetAllMilestonesForRepoAsync(int repoId)
        {
            Repository repo = _context.Repositories.Where(repo => repo.Id == repo.Id).FirstOrDefault();
            List<Milestone> milestones = _context.Milestones.Where(milestone => milestone.RepositoryId == repo.Id).ToList();

            if (repo != null)
            {
                foreach (Milestone milestone in milestones)
                { 
                if (milestone.CommitThreshold <= repo.Commits.Count)
                    {
                        milestone.IsCompleted = true;
                    }
                }
                return _context.Milestones.Where(milestone => milestone.RepositoryId == repo.Id).ToList();
            }
            return null;
        }

        public async Task<Milestone> DeleteMilestoneAsync(int milestoneId)
        {
            Milestone milestoneToDelete = _context.Milestones.Where(milestone => milestone.Id == milestoneId).FirstOrDefault();
            if (milestoneToDelete != null)
            {
                _context.Milestones.Remove(milestoneToDelete);
                await _context.SaveChangesAsync();
                return milestoneToDelete;
            }
            return null;
        }

        public async Task<Milestone> GetMilestone(int milestoneId)
        { 
        var milestone = _context.Milestones.Where(milestone => milestone.Id == milestoneId).FirstOrDefault();
            if (milestone != null)
            {
                return milestone;
            }
            return null;
        }
        public async Task<Milestone> CompleteMilestone(int milestoneId)
        {
            var milestone = await GetMilestone(milestoneId);

            if (milestone != null)
            {
                milestone.IsCompleted = true;
                await _context.SaveChangesAsync();
                return milestone;
            }
            return null;
        }
    }
}
