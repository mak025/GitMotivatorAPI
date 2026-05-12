namespace GithubMotivator.Models
{
    public class Milestone
    {
        public int Id { get; set; }
        public int CommitThreshold { get; set; }
        public int TargetThreshold { get; set; }
        public bool IsCompleted { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public int RepositoryId { get; set; }
        public Repository Repository { get; set; } = null!;
    }
}
