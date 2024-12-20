using System;
using System.Collections.Generic;

namespace FocusLearn.Models.Domain;

public partial class IoTSession
{
    public int SessionId { get; set; }

    public int UserId { get; set; }

    public int MethodId { get; set; }

    public string? SessionType { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int? Duration { get; set; }

    public virtual ConcentrationMethod Method { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
