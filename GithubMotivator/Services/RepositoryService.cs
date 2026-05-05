using GithubMotivator.Data;
using GithubMotivator.Models;
using GithubMotivator.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GithubMotivator.Services
{
    public class RepositoryService : IRepositoryService
    {
        private readonly AppDbContext _context;
        private readonly IGithubService _githubService;

        public RepositoryService(AppDbContext context, IGithubService githubService)
        {
            _context = context;
            _githubService = githubService;
        }

        public async Task<Repository> TrackRepositoryAsync(string url, string token)
        {
            // Simple URL parsing: https://github.com/owner/repo
            var parts = url.TrimEnd('/').Split('/');
            if (parts.Length < 2) throw new ArgumentException("Invalid GitHub URL");
            
            string name = parts[^1];
            string owner = parts[^2];

            var repo = await _context.Repositories
                .FirstOrDefaultAsync(r => r.Owner == owner && r.Name == name);

            if (repo == null)
            {
                repo = new Repository
                {
                    Owner = owner,
                    Name = name,
                    Url = url
                };
                _context.Repositories.Add(repo);
                await _context.SaveChangesAsync();
            }

            // Trigger initial sync
            await SyncCommitsAsync(repo.Id, token);
            
            return repo;
        }

        public async Task SyncCommitsAsync(int repositoryId, string token)
        {
            var repo = await _context.Repositories
                .Include(r => r.Commits)
                .FirstOrDefaultAsync(r => r.Id == repositoryId);
                
            if (repo == null) return;

            // Fetch commits since last fetch or all if never fetched
            var commits = await _githubService.FetchCommitsAsync(repo.Owner, repo.Name, token, repo.LastFetchedAt);

            // Get existing SHAs to avoid duplicates (though DB has unique constraint as backup)
            var existingShas = await _context.Commits
                .Where(c => c.RepositoryId == repositoryId)
                .Select(c => c.Sha)
                .ToListAsync();
            var existingShaSet = new HashSet<string>(existingShas);

            var newCommits = commits
                .Where(c => !existingShaSet.Contains(c.Sha))
                .ToList();

            foreach (var commit in newCommits)
            {
                commit.RepositoryId = repositoryId;
                _context.Commits.Add(commit);
            }

            repo.LastFetchedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task<DashboardStatsDTO> GetDashboardStatsAsync(int? repositoryId = null)
        {
            Repository? repo;
            if (repositoryId.HasValue)
            {
                repo = await _context.Repositories
                    .Include(r => r.Commits)
                    .FirstOrDefaultAsync(r => r.Id == repositoryId.Value);
            }
            else
            {
                // Default to the first one for now as per "just one for now"
                repo = await _context.Repositories
                    .Include(r => r.Commits)
                    .OrderByDescending(r => r.Id)
                    .FirstOrDefaultAsync();
            }

            if (repo == null) return new DashboardStatsDTO();

            var totalCommits = repo.Commits.Count;
            
            return new DashboardStatsDTO
            {
                RepositoryUrl = repo.Url,
                TotalCommits = totalCommits,
                Milestone = CalculateMilestone(totalCommits),
                Leaderboard = repo.Commits
                    .GroupBy(c => new { c.AuthorEmail, c.AuthorName })
                    .Select(g => new LeaderboardEntry
                    {
                        AuthorName = g.Key.AuthorName,
                        AuthorEmail = g.Key.AuthorEmail,
                        CommitCount = g.Count()
                    })
                    .OrderByDescending(l => l.CommitCount)
                    .ToList()
            };
        }

        public async Task SyncAllRepositoriesAsync()
        {
            var repos = await _context.Repositories.ToListAsync();
            // We need a token. Let's get the token from the first user who has one.
            var userWithToken = await _context.Users
                .Where(u => !string.IsNullOrEmpty(u.GitHubToken))
                .FirstOrDefaultAsync();
                
            if (userWithToken == null || string.IsNullOrEmpty(userWithToken.GitHubToken)) return;

            foreach (var repo in repos)
            {
                await SyncCommitsAsync(repo.Id, userWithToken.GitHubToken);
            }
        }

        private MilestoneProgress CalculateMilestone(int totalCommits)
        {
            int[] thresholds = { 0, 15, 30, 50, 100, 200, 400, 800, 1600 };
            int level = 0;
            int nextThreshold = thresholds[1];

            for (int i = 0; i < thresholds.Length - 1; i++)
            {
                if (totalCommits >= thresholds[i])
                {
                    level = i;
                    nextThreshold = thresholds[i + 1];
                }
                else
                {
                    break;
                }
            }
            
            // If we exceeded all thresholds
            if (totalCommits >= thresholds[^1])
            {
                level = thresholds.Length - 1;
                nextThreshold = totalCommits + 1; // Or some other logic
            }

            int currentLevelStart = thresholds[level];
            int commitsInThisLevel = totalCommits - currentLevelStart;
            int totalNeededInThisLevel = nextThreshold - currentLevelStart;
            
            double percentage = (double)commitsInThisLevel / totalNeededInThisLevel * 100;
            if (percentage > 100) percentage = 100;

            return new MilestoneProgress
            {
                CurrentLevel = level,
                CommitsInCurrentLevel = totalCommits, // As requested: total amount of commits
                TargetForNextLevel = nextThreshold,
                Percentage = Math.Round(percentage, 2)
            };
        }
    }
}
