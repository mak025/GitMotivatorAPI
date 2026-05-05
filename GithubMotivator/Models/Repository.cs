using System.ComponentModel.DataAnnotations;

namespace GithubMotivator.Models
{
    public class Repository
    {
        public int Id { get; set; }
        
        [Required]
        public string Owner { get; set; } = string.Empty;
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string Url { get; set; } = string.Empty;
        
        public DateTime? LastFetchedAt { get; set; }
        
        public ICollection<Commit> Commits { get; set; } = new List<Commit>();
    }
}