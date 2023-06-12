using System;
using System.Collections.Generic;

namespace EF_Test;

public partial class EdsSetMetricType
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public bool IsRepetition { get; set; }

    public bool IsWeight { get; set; }

    public bool IsResistance { get; set; }

    public bool IsDistance { get; set; }

    public bool IsTime { get; set; }

    public virtual ICollection<EdsAvaiableSetMetric> EdsAvaiableSetMetrics { get; } = new List<EdsAvaiableSetMetric>();

    public virtual ICollection<EdsSetMetricsDefault> EdsSetMetricsDefaults { get; } = new List<EdsSetMetricsDefault>();
}
