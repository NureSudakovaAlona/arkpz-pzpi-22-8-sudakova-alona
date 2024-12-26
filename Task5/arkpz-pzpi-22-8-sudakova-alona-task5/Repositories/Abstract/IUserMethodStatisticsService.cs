using FocusLearn.Models.Domain;
using FocusLearn.Models.DTO;

namespace FocusLearn.Repositories.Abstract
{
    public interface IUserMethodStatisticsService
    {
        Task<IEnumerable<UserMethodStatistics>> GetAllStatisticsAsync();
        Task<UserMethodStatistics?> GetStatisticsByIdAsync(int id);

    }
}
