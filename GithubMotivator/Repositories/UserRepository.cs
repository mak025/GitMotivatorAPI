using GithubMotivator.Data;
using GithubMotivator.Models;
using Microsoft.EntityFrameworkCore;

namespace GithubMotivator.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        return await _db.Users.ToListAsync();
    }

    public async Task<User?> GetByUsername(string username)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
    }
}
