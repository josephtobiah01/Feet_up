using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParentMiddleWare.Models
{
    public  class EmDailyPlan
    {
        public long Id { get; set; }

        public long FkEdsWeeklyPlanId { get; set; }

        public DateTime? StartDay { get; set; }

        public DateTime? EndDay { get; set; }

        public bool IsComplete { get; set; }
    }
}
