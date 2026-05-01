using GithubMotivator.Models;

namespace GithubMotivator.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAll();
    Task<User?> GetByUsername(string username);
}
