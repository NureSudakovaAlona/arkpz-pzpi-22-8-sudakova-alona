using FocusLearn.Models.Domain;
using FocusLearn.Models.DTO;
using FocusLearn.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace FocusLearn.Repositories.Implementation
{
    public class AssignmentService : IAssignmentService
    {
        private readonly FocusLearnDbContext _context;

        public AssignmentService(FocusLearnDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Отримати всі завдання
        /// </summary>
        public async Task<IEnumerable<AssignmentDTO>> GetAllAssignmentsAsync()
        {
            return await _context.Assignments
                .Select(a => new AssignmentDTO
                {
                    Title = a.Title,
                    Description = a.Description,
                    FileLink = a.FileLink,
                    StudentId = a.StudentId,
                    TutorId = a.TutorId,
                    TaskId = a.TaskId,
                    Status = a.Status,
                    DueDate = a.DueDate,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt,
                    Rating = a.Rating
                })
                .ToListAsync();
        }

        /// <summary>
        /// Отримати завдання за ID
        /// </summary>
        public async Task<AssignmentDTO?> GetAssignmentByIdAsync(int id)
        {
            return await _context.Assignments
                .Where(a => a.AssignmentId == id)
                .Select(a => new AssignmentDTO
                {
                    Title = a.Title,
                    Description = a.Description,
                    FileLink = a.FileLink,
                    StudentId = a.StudentId,
                    TutorId = a.TutorId,
                    TaskId = a.TaskId,
                    Status = a.Status,
                    DueDate = a.DueDate,
                    UpdatedAt = a.UpdatedAt,
                    Rating = a.Rating
                })
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Додати завдання
        /// </summary>
        public async Task AddAssignmentAsync(int tutorId, AssignmentDTO assignmentDto)
        {
            var assignment = new Assignment
            {
                Title = assignmentDto.Title,
                Description = assignmentDto.Description,
                FileLink = assignmentDto.FileLink,
                StudentId = assignmentDto.StudentId,
                TutorId = assignmentDto.TutorId,
                TaskId = assignmentDto.TaskId,
                Status = "Pending",
                DueDate = assignmentDto.DueDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Редагувати завдання
        /// </summary>
        public async Task<bool> UpdateAssignmentAsync(int id, AssignmentDTO assignmentDto)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null) return false;

            assignment.Title = assignmentDto.Title ?? assignment.Title;
            assignment.Description = assignmentDto.Description ?? assignment.Description;
            assignment.FileLink = assignmentDto.FileLink ?? assignment.FileLink;
            assignment.StudentId = assignmentDto.StudentId ?? assignment.StudentId;
            assignment.TaskId = assignmentDto.TaskId ?? assignment.TaskId;
            assignment.Status = assignmentDto.Status ?? assignment.Status;
            assignment.DueDate = assignmentDto.DueDate ?? assignment.DueDate;
            assignment.Rating = assignmentDto.Rating ?? assignment.Rating;
            assignment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Видалити завдання
        /// </summary>
        public async Task<bool> DeleteAssignmentAsync(int id)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null) return false;

            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
