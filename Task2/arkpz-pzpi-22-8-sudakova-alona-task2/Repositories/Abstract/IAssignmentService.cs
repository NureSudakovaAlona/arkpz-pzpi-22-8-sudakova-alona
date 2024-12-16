using FocusLearn.Models.DTO;

namespace FocusLearn.Repositories.Abstract
{
    public interface IAssignmentService
    {
        Task<IEnumerable<AssignmentDTO>> GetAllAssignmentsAsync();
        Task<AssignmentDTO?> GetAssignmentByIdAsync(int id);
        Task AddAssignmentAsync(int creatorId, AssignmentDTO assignmentDto);
        Task<bool> UpdateAssignmentAsync(int id, AssignmentDTO assignmentDto);
        Task<bool> DeleteAssignmentAsync(int id);
    }
}
