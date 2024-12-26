using System;
using System.Collections.Generic;

namespace FocusLearn.Models.Domain;

public partial class LearningMaterial
{
    public int MaterialId { get; set; }

    public int CreatorId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? FileLink { get; set; }

    public DateTime? AddedAt { get; set; }

    public virtual User Creator { get; set; } = null!;

    public virtual ICollection<Assignment> AssignmentLearningMaterials { get; } = new List<Assignment>();

}
