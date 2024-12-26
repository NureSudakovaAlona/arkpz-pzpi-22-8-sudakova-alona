using FocusLearn.Models.Domain;
using FocusLearn.Models.DTO;
using FocusLearn.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace FocusLearn.Repositories.Implementation
{
    public class UserMethodStatisticsService : IUserMethodStatisticsService
    {
        private readonly FocusLearnDbContext _context;

        public UserMethodStatisticsService(FocusLearnDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Отримати всю статистику
        /// </summary>
        public async Task<IEnumerable<UserMethodStatistics>> GetAllStatisticsAsync()
        {
            return await _context.UserMethodStatistics.ToListAsync();
        }

        /// <summary>
        /// Отримати статистику за ID
        /// </summary>
        public async Task<UserMethodStatistics?> GetStatisticsByIdAsync(int id)
        {
            return await _context.UserMethodStatistics.FindAsync(id);
        }
        
    }
}
