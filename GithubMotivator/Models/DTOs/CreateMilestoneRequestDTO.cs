namespace GithubMotivator.Models.DTOs
{
    public class CreateMilestoneRequestDTO
    {
        public int CommitThreshold { get; set; }
        public string Message { get; set; } = string.Empty;
        public int RepositoryId { get; set; }
    }
}
