using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParentMiddleWare.Models
{
    public class NdSupplementPlanSupplement
    {
        public long Id { get; set; }

        public long FkSupplementPlanDaily { get; set; }

        public long FkSupplementReference { get; set; }

        public bool IsCustomerCreatedEntry { get; set; }

        public bool IsFreeEntry { get; set; }

        public string FreeEntryName { get; set; }

        public long? FkFreeEntryUnitMetric { get; set; }

        public string Remark { get; set; }

        public long? FkReferenceToTrainer { get; set; }

      //  public virtual NdsSupplementPlanDaily FkSupplementPlanDailyNavigation { get; set; }

        public virtual NdSupplementReference FkSupplementReferenceNavigation { get; set; }

        public virtual ICollection<NdSupplementPlanDose> NdsSupplementPlanDose { get; set; } = new List<NdSupplementPlanDose>();
    }
}
