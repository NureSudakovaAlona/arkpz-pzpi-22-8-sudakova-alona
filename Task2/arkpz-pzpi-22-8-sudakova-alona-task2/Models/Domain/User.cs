using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace FocusLearn.Models.Domain;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Role { get; set; }

    public string? ProfilePhoto { get; set; }

    public string? Language { get; set; }

    public string? ProfileStatus { get; set; }

    public string? AuthProvider { get; set; }

    public string? ProviderId { get; set; }

    public DateTime? RegistrationDate { get; set; }

    public virtual ICollection<Assignment> AssignmentStudents { get; } = new List<Assignment>();

    public virtual ICollection<Assignment> AssignmentTutors { get; } = new List<Assignment>();

    public virtual ICollection<IoTsession> IoTsessions { get; } = new List<IoTsession>();

    public virtual ICollection<LearningMaterial> LearningMaterials { get; } = new List<LearningMaterial>();

    public virtual ICollection<UserMethodStatistic> UserMethodStatistics { get; } = new List<UserMethodStatistic>();
}
