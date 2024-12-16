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

    public virtual ICollection<IoTsession> IoTsessions { get; } = new List<IoTsession>();

    public virtual ICollection<UserMethodStatistic> UserMethodStatistics { get; } = new List<UserMethodStatistic>();
}
