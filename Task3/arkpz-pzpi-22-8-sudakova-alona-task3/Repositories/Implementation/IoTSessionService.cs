using FocusLearn.Models.Domain;
using FocusLearn.Models.DTO;
using FocusLearn.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace FocusLearn.Repositories.Implementation
{
    public class IoTSessionService : IIoTSessionService
    {
        private readonly FocusLearnDbContext _context;

        public IoTSessionService(FocusLearnDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Додавання нової IoT-сесії
        /// </summary>
        public async Task<bool> AddIoTSessionAsync(IoTSessionDTO sessionDto)
        {
            var session = new IoTSession
            {
                UserId = sessionDto.UserId,
                MethodId = sessionDto.MethodId,
                SessionType = sessionDto.SessionType,
                StartTime = sessionDto.StartTime,
                EndTime = sessionDto.EndTime
            };

            _context.IoTSessions.Add(session);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Отримання IoT-сесій користувача
        /// </summary>
        public async Task<IEnumerable<IoTSessionDTO>> GetIoTSessionsByUserIdAsync(int userId)
        {
            return await _context.IoTSessions
                .Where(s => s.UserId == userId)
                .Select(s => new IoTSessionDTO
                {
                    UserId = s.UserId,
                    MethodId = s.MethodId,
                    SessionType = s.SessionType,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime
                })
                .ToListAsync();
        }
    }
}

