using System;
using System.Collections.Generic;

namespace FocusLearn.Models.Domain;

public partial class ConcentrationMethod
{
    public int MethodId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int WorkDuration { get; set; }

    public int BreakDuration { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<IoTSession> IoTSessions { get; } = new List<IoTSession>();

    public virtual ICollection<UserMethodStatistics> UserMethodStatistics { get; } = new List<UserMethodStatistics>();
}
