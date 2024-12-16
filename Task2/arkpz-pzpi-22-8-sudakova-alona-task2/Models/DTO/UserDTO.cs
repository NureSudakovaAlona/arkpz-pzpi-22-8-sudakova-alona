namespace FocusLearn.Models.DTO
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? ProfilePhoto { get; set; }
        public string? Language { get; set; }
        public string? Role { get; set; }
        public string? ProfileStatus { get; set; }
    }
}
