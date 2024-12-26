namespace FocusLearn.Models.DTO
{
    public class MethodUsageStatisticsDTO
    {
        /// <summary>
        /// DTO для статистики використання кожної методики концентрації окремо
        /// </summary>
        public int MethodId { get; set; }
        public int TotalConcentrationTime { get; set; }
        public int BreakCount { get; set; }
        public int MissedBreaks { get; set; }
    }
}
