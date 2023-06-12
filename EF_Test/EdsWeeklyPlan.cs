using System;
using System.Collections.Generic;

namespace EF_Test;

public partial class EdsWeeklyPlan
{
    public long Id { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public long FkEds12weekPlan { get; set; }

    public virtual ICollection<EdsDailyPlan> EdsDailyPlans { get; } = new List<EdsDailyPlan>();

    public virtual Eds12weekPlan FkEds12weekPlanNavigation { get; set; } = null!;
}
