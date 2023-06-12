using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParentMiddleWare.Models
{
    public class NdSupplementReference
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long FkUnitMetric { get; set; }

        public long FkSupplementInstruction { get; set; }

        public string InstructionText { get; set; }

        public virtual NdSupplementInstruction FkSupplementInstructionNavigation { get; set; }

        public virtual NdUnitMetric FkUnitMetricNavigation { get; set; }

      //  public virtual ICollection<NdsSupplementCustomerInventory> NdsSupplementCustomerInventory { get; set; } = new List<NdsSupplementCustomerInventory>();

     //   public virtual ICollection<NdsSupplementLegalStatus> NdsSupplementLegalStatus { get; set; } = new List<NdsSupplementLegalStatus>();

        public virtual ICollection<NdSupplementPlanSupplement> NdsSupplementPlanSupplement { get; set; } = new List<NdSupplementPlanSupplement>();
    }
}
