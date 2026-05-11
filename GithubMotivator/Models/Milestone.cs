namespace GithubMotivator.Models
{
    public class Milestone
    {
        public int Id { get; set; }
        public int CommitTreshold { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
