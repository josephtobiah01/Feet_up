using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParentMiddleWare.Models
{
    public class NdSupplementList
    {
        public long SupplmentDoseId { get; set; }
        public string SupplementName { get; set; }
        public String TimeString { get; set; }
        public List<DayOfWeek> DayOfWeek { get; set; }
        public float Ammount { get; set; }  // i.e "1"
        public string Type { get; set; }  // i/e "tablet"
        public string Frequency { get; set; }
        public string TimeRemark { get; set; }
        public string FoodRemark { get; set; }

    }

    public enum DayOfWeek
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }
}
