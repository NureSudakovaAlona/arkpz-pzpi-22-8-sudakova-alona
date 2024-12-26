namespace FocusLearn.Models.DTO
{
    public class AssignmentDTO
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? FileLink { get; set; }
        public int? StudentId { get; set; }
        public int TutorId { get; set; }
        public int? TaskId { get; set; }
        public string? Status { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Rating { get; set; }
    }
}
