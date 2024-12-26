using FocusLearn.Models.Domain;
using FocusLearn.Models.DTO;
using FocusLearn.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace FocusLearn.Repositories.Implementation
{
    public class BusinessLogicService : IBusinessLogicService
    {
        private readonly FocusLearnDbContext _context;

        public BusinessLogicService(FocusLearnDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Обчислення загальної статистики продуктивності користувача
        /// </summary>
        public async Task<UserStatisticsDTO> CalculateUserStatisticsAsync(int userId, DateTime periodStartDate, string periodType)
        {
            var periodEndDate = GetPeriodEndDate(periodStartDate, periodType);

            // Перевірка: чи пройшов достатній період
            if (DateTime.UtcNow < periodEndDate)
            {
                throw new InvalidOperationException("Недостатньо даних для обраного періоду.");
            }

            var sessions = await _context.IoTSessions
                .Where(s => s.UserId == userId &&
                            s.StartTime >= periodStartDate &&
                            s.SessionType == "Concentration")
                .ToListAsync();

            var totalConcentrationTime = sessions.Sum(s => s.Duration ?? 0);
            var breakCount = await _context.IoTSessions
                .CountAsync(s => s.UserId == userId &&
                                 s.StartTime >= periodStartDate &&
                                 s.SessionType == "Break");

            var missedBreaks = CalculateMissedBreaks(sessions, breakCount);

            return new UserStatisticsDTO
            {
                TotalConcentrationTime = totalConcentrationTime,
                BreakCount = breakCount,
                MissedBreaks = missedBreaks
            };
        }

        /// <summary>
        /// Оновлення статистики для методик
        /// </summary>
        public async Task UpdateMethodStatisticsAsync(int userId, int methodId, DateTime periodStartDate, string periodType)
        {
            var periodEndDate = GetPeriodEndDate(periodStartDate, periodType);

            // Отримуємо сесії для заданого користувача, методики та періоду
            var sessions = await _context.IoTSessions
                .Where(s => s.UserId == userId &&
                            s.MethodId == methodId &&
                            s.StartTime >= periodStartDate &&
                            s.StartTime < periodEndDate)
                .ToListAsync();

            if (!sessions.Any()) return;

            var totalConcentrationTime = sessions
                .Where(s => s.SessionType == "Concentration" && s.Duration.HasValue)
                .Sum(s => s.Duration.Value);

            var breakCount = sessions.Count(s => s.SessionType == "Break");
            var expectedBreaks = CalculateExpectedBreaks(sessions);
            var missedBreaks = Math.Max(0, expectedBreaks - breakCount);

            var existingStatistic = await _context.UserMethodStatistics
                .FirstOrDefaultAsync(s => s.UserId == userId &&
                                          s.MethodId == methodId &&
                                          s.PeriodStartDate == periodStartDate);

            if (existingStatistic == null)
            {
                var newStatistic = new UserMethodStatistics
                {
                    UserId = userId,
                    MethodId = methodId,
                    PeriodStartDate = periodStartDate,
                    PeriodType = periodType,
                    TotalConcentrationTime = totalConcentrationTime,
                    BreakCount = breakCount,
                    MissedBreaks = missedBreaks,
                    LastUpdated = DateTime.UtcNow
                };

                _context.UserMethodStatistics.Add(newStatistic);
            }
            else
            {
                existingStatistic.TotalConcentrationTime = totalConcentrationTime;
                existingStatistic.BreakCount = breakCount;
                existingStatistic.MissedBreaks = missedBreaks;
                existingStatistic.LastUpdated = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Визначити найефективнішу методику за період
        /// </summary>
        public async Task<string> GetMostEffectiveMethodAsync(int userId, DateTime periodStartDate, string periodType)
        {
            var periodEndDate = GetPeriodEndDate(periodStartDate, periodType);

            // Отримуємо записи статистики для конкретного користувача та періоду
            var statistics = await _context.UserMethodStatistics
                .Where(s => s.UserId == userId &&
                            s.PeriodStartDate >= periodStartDate &&
                            s.PeriodStartDate < periodEndDate)
                .Include(s => s.Method)
                .ToListAsync();

            if (!statistics.Any())
                return "Немає достатньо даних для визначення найефективнішої методики.";

            // Визначаємо найефективнішу методику
            var mostEffectiveMethod = statistics
                .OrderByDescending(s => s.TotalConcentrationTime)
                .ThenBy(s => s.MissedBreaks) // Менша кількість пропущених перерв
                .FirstOrDefault();

            return mostEffectiveMethod != null
                ? $"У період з {periodStartDate:yyyy-MM-dd} до {periodEndDate:yyyy-MM-dd} найефективнішою методикою концентрації була '{mostEffectiveMethod.Method?.Title}'."
                : "Не вдалося визначити найефективнішу методику.";
        }


        /// <summary>
        /// Розрахунок очікуваних перерв
        /// </summary>
        private int CalculateExpectedBreaks(IEnumerable<IoTSession> sessions)
        {
            return sessions.Count(s => s.SessionType == "Concentration") / 2;
        }
        /// <summary>
        /// Розрахунок пропущених перерв
        /// </summary>
        private int CalculateMissedBreaks(IEnumerable<IoTSession> sessions, int actualBreakCount)
        {
            var expectedBreakCount = CalculateExpectedBreaks(sessions);
            return Math.Max(0, expectedBreakCount - actualBreakCount);
        }

        /// <summary>
        /// Обчислення кінцевої дати періоду
        /// </summary>
        private DateTime GetPeriodEndDate(DateTime periodStartDate, string periodType)
        {
            return periodType.ToLower() switch
            {
                "day" => periodStartDate.AddDays(1),
                "week" => periodStartDate.AddDays(7),
                "month" => periodStartDate.AddMonths(1),
                _ => throw new ArgumentException("Невірний тип періоду. Допустимі значення: 'Day', 'Week', 'Month'.")
            };
        }
    }
}
