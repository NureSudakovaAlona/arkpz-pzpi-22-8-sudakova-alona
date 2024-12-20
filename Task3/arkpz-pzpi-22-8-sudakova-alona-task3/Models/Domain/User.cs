using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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

    [JsonIgnore]
    public virtual ICollection<Assignment> AssignmentStudents { get; } = new List<Assignment>();
    [JsonIgnore]
    public virtual ICollection<Assignment> AssignmentTutors { get; } = new List<Assignment>();
    [JsonIgnore]
    public virtual ICollection<IoTSession> IoTSessions { get; } = new List<IoTSession>();
    [JsonIgnore]
    public virtual ICollection<LearningMaterial> LearningMaterials { get; } = new List<LearningMaterial>();
    [JsonIgnore]
    public virtual ICollection<UserMethodStatistics> UserMethodStatistics { get; } = new List<UserMethodStatistics>();
}
