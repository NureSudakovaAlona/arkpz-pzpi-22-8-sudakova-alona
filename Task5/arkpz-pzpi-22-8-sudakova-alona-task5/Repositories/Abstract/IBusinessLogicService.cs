using FocusLearn.Models.DTO;

namespace FocusLearn.Repositories.Abstract
{
    public interface IBusinessLogicService
    {
        Task<UserStatisticsDTO> CalculateUserStatisticsAsync(int userId, DateTime periodStartDate, string periodType);
        Task<string> GetMostEffectiveMethodAsync(int userId, DateTime periodStartDate, string periodType);
        Task UpdateMethodStatisticsAsync(int userId, int methodId, DateTime periodStartDate, string periodType);
    }
}
