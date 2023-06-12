using System;
using System.Collections.Generic;

namespace EF_Test;

public partial class EdsExercise
{
    public long Id { get; set; }

    public long FkTrainingId { get; set; }

    public long FkExerciseTypeId { get; set; }

    public bool IsSkipped { get; set; }

    public bool IsComplete { get; set; }

    public DateTime? EndTimeStamp { get; set; }

    public bool IsCustomerAddedExercise { get; set; }

    public virtual ICollection<EdsSet> EdsSets { get; } = new List<EdsSet>();

    public virtual EdsTrainingSession FkTraining { get; set; } = null!;
}
