using System.Text.Json.Serialization;

namespace GithubMotivator.Models
{
    public class Milestone
    {
        public int Id { get; set; }
        public int CommitThreshold { get; set; }
        public bool IsCompleted { get; set; } = false;
        public string Message { get; set; } = string.Empty;

        [JsonIgnore]
        public Repository? Repository { get; set; }
        public int RepositoryId { get; set; }
    }
}
