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
        public UserType Type { get; set; }
        public int Commits { get; set; }
        public int PullRequests { get; set; }
        public int Merges { get; set; }
        public int Reviews { get; set; }

        public User()
        {
        }

        public override string ToString()
        {
            return $"{{Id: {Id}, Name: {Name}, Username: {Username}, Email: {Email}, Commits: {Commits}, PullRequests: {PullRequests}, Merges: {Merges}, Reviews: {Reviews}}}";
        }
    }
}
