namespace FocusLearn.Models.DTO
{
    public class IoTSessionDTO
    {
        public int UserId { get; set; }
        public int MethodId { get; set; }
        public string SessionType { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}

