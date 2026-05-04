using GithubMotivator.Interfaces;
using GithubMotivator.Models;
using GithubMotivator.Data;
namespace GithubMotivator.Repos
{
    public class StatisticsRepo : IStatisticsRepo
    {
        private readonly AppDbContext _context;
        public StatisticsRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Statistics>> GetAll()
        {
            return _context.Statistics.ToList();
        }
        public async Task<Statistics?> Get(int id)
        { 
        return _context.Statistics.FirstOrDefault(s => s.Id == id);
        }
        public async Task<Statistics?> Add(Statistics stats)
        { 
        return _context.Statistics.Add(stats).Entity;
        }
        public async Task<Statistics?> Delete(int id)
        { 
        return _context.Statistics.Remove(_context.Statistics.FirstOrDefault(s => s.Id == id)).Entity;
        }
        public async Task<Statistics?> Update(int id, Statistics updatedStats)
        {
            var stats = _context.Statistics.FirstOrDefault(stats => stats.Id == id);
            if (stats != null)
            {
                stats.PullRequestsTotal = updatedStats.PullRequestsTotal;
                stats.CommitsTotal = updatedStats.CommitsTotal;
                stats.MergesTotal = updatedStats.MergesTotal;
                stats.ReviewsTotal = updatedStats.ReviewsTotal;
                stats.ContributorsTotal = updatedStats.ContributorsTotal;
                _context.SaveChanges();
                return stats;
            }
            return null;
        }
    }
}
