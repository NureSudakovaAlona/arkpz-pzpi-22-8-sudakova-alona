using System;
using System.Collections.Generic;

namespace FocusLearn.Models.Domain;

public partial class Assignment
{
    public int AssignmentId { get; set; }
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

    public virtual User? Student { get; set; }
    public virtual LearningMaterial? Task { get; set; }
    public virtual User Tutor { get; set; } = null!;
}
