namespace FocusLearn.Models.DTO
{
    public class ConcentrationMethodDTO
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int WorkDuration { get; set; }
        public int BreakDuration { get; set; }

    }
}
