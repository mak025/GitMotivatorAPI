using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Octokit;

namespace GithubMotivator.Models
{
    public class Commit
    {
        public int Id { get; set; }
        
        [Required]
        public string Sha { get; set; } = string.Empty;
        
        public string Message { get; set; } = string.Empty;
        
        [Required]
        public string AuthorName { get; set; } = string.Empty;
        
        [Required]
        public string AuthorEmail { get; set; } = string.Empty;
        
        public DateTime Date { get; set; }
        
        public int RepositoryId { get; set; }
        
        [JsonIgnore]
        public Repository? Repository { get; set; }
    }
}