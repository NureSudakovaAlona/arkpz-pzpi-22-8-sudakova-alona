using System;
using System.Collections.Generic;

namespace FocusLearn.Models.Domain;

public partial class UserMethodStatistic
{
    public int StatisticId { get; set; }

    public int UserId { get; set; }

    public int MethodId { get; set; }

    public DateTime PeriodStartDate { get; set; }

    public string PeriodType { get; set; } = null!;

    public int TotalConcentrationTime { get; set; }

    public int BreakCount { get; set; }

    public int MissedBreaks { get; set; }

    public DateTime LastUpdated { get; set; }

    public virtual ConcentrationMethod Method { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
