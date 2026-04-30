namespace GithubMotivator.Models
{
    public class User
    {
        public enum UserType
        {
            Manager, Contributor
        }
        string _gitHubProps; //not done yet
        public string Name { get; set; }
        public int Id { get; set; }
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
            return "${Id: {Id}, Name: {Name}, {Commits: {Commits}, PullRequests: {PullRequests}, Merges: {Merges}, Reviews: {Reviews}}}";
        }
    }
}
