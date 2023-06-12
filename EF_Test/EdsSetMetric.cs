using System;
using System.Collections.Generic;

namespace EF_Test;

public partial class EdsSetMetric
{
    public long Id { get; set; }

    public long? FkSetId { get; set; }

    public long? FkMetricsTypeId { get; set; }

    public double? TargetCustomMetric { get; set; }

    public double? ActualCustomMetric { get; set; }

    public virtual EdsSet? FkSet { get; set; }
}
