using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParentMiddleWare.Models
{
    public class NdSupplementPlanDose
    {
        public long Id { get; set; }

        public long FkSupplementPlanSupplement { get; set; }

        public double UnitCount { get; set; }

        public DateTime ScheduledTime { get; set; }

        public string Remark { get; set; }

        public double? DoseWarningLimit { get; set; }

        public double? DoseHardCeilingLimit { get; set; }

        public virtual NdSupplementPlanSupplement FkSupplementPlanSupplementNavigation { get; set; }
    }
}
