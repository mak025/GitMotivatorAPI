namespace GithubMotivator.Models.DTOs
{
    public class DashboardStatsDTO
    {
        public string RepositoryUrl { get; set; } = string.Empty;
        public int TotalCommits { get; set; }
        public MilestoneProgress Milestone { get; set; } = new();
        public List<LeaderboardEntry> Leaderboard { get; set; } = new();
    }

    public class MilestoneProgress
    {
        public int CurrentLevel { get; set; }
        public int CommitsInCurrentLevel { get; set; }
        public int TargetForNextLevel { get; set; }
        public double Percentage { get; set; }
    }

    public class LeaderboardEntry
    {
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorEmail { get; set; } = string.Empty;
        public int CommitCount { get; set; }
    }
}