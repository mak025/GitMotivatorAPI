namespace GithubMotivator.Models
{
    public class Milestone
    {
        public int Id { get; set; }
        public int CommitThreshold { get; set; }
        public bool IsCompleted { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public Repository RepositoryId { get; set; }
    }
}
