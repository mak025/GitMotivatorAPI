using GithubMotivator.Data;
using GithubMotivator.Models;
using Microsoft.EntityFrameworkCore;

namespace GithubMotivator.Services
{
    public class MilestoneService : IMilestoneService
    {
        private readonly AppDbContext _context;
        //get repo id -> set var = repo.Id -> use var to get milestones for that repo

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
            var repo = await _context.Repositories
                .Include(r => r.Commits)
                .FirstOrDefaultAsync(r => r.Id == repoId);

            if (repo == null)
            {
                return null;
            }

            var milestones = await _context.Milestones.Where(milestone => milestone.RepositoryId == repoId).ToListAsync();
            
            //flip bool if commits => CommitThreshold
            foreach (var milestone in milestones)
            {
                if (repo.Commits.Count >= milestone.CommitThreshold)
                {
                    milestone.IsCompleted = true;
                }
            }
            
            return milestones;
        }

        public async Task<Milestone> DeleteMilestoneAsync(int milestoneId)
        {
            Milestone milestoneToDelete = await _context.Milestones.Where(milestone => milestone.Id == milestoneId).FirstOrDefaultAsync();
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
        var milestone = await _context.Milestones.Where(milestone => milestone.Id == milestoneId).FirstOrDefaultAsync();
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
