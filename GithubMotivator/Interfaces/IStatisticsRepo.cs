using GithubMotivator.Models;
namespace GithubMotivator.Interfaces
{
    public interface IStatisticsRepo
    {
        IEnumerable<Statistics> GetAll();
        Statistics? Get(int id);
        Statistics? Add(Statistics stats);
        Statistics? Delete(int id);
        Statistics? Update(int id, Statistics updatedStats);
    }
}
