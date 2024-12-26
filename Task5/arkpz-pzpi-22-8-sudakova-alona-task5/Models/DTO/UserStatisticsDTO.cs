namespace FocusLearn.Models.DTO
{
    public class UserStatisticsDTO
    {
        /// <summary>
        /// DTO для передачі загальної статистики продуктивності користувача
        /// </summary>
        public int TotalConcentrationTime { get; set; }
        public int BreakCount { get; set; }
        public int MissedBreaks { get; set; }
    }
}
