using System;
using System.Collections.Generic;

namespace EF_Test;

public partial class EdsSet
{
    public long Id { get; set; }

    public long? FkExerciseId { get; set; }

    public short SetSequenceNumber { get; set; }

    public bool IsComplete { get; set; }

    public bool IsSkipped { get; set; }

    public bool IsCustomerAddedSet { get; set; }

    public DateTime? EndTimeStamp { get; set; }

    public virtual ICollection<EdsSetMetric> EdsSetMetrics { get; } = new List<EdsSetMetric>();

    public virtual EdsExercise? FkExercise { get; set; }
}
