using System;
using System.Collections.Generic;

namespace EF_Test;

public partial class EdsAvaiableSetMetric
{
    public long Id { get; set; }

    public long? FkExerciseTypeId { get; set; }

    public long? FkSetMetricsTypesId { get; set; }

    public bool IsDeleted { get; set; }

    public virtual EdsExerciseType? FkExerciseType { get; set; }

    public virtual EdsSetMetricType? FkSetMetricsTypes { get; set; }
}
