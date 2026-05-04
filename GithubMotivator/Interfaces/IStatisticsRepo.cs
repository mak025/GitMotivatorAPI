using GithubMotivator.Models;
namespace GithubMotivator.Interfaces
{
    public interface IStatisticsRepo
    {
        Task<IEnumerable<Statistics>> GetAll();
        Task<Statistics?> Get(int id);
        Task<Statistics?> Add(Statistics stats);
        Task<Statistics?> Delete(int id);
        Task<Statistics?> Update(int id, Statistics updatedStats);
    }
}
