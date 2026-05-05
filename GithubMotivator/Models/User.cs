using Octokit;

namespace GithubMotivator.Models
{
    public class User
    {
        public enum UserType
        {
            Manager, Contributor
        }
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? GitHubToken { get; set; }
        public UserType Type { get; set; }
        
        //From Octokit (Github SDK)
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public Repository? Repository { get; set; }
        public int Commits { get; set; }

        public User()
        {
        }

        public override string ToString()
        {
            return $"{{Id: {Id}, Name: {Name}, Username: {Username}, Email: {Email}, Commits: {Commits}}}";
        }
    }
}