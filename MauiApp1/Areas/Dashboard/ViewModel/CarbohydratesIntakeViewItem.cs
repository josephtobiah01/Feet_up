using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Areas.Dashboard.ViewModel
{
    public class CarbohydratesIntakeViewItem
    {
        public Guid CarbohydratesDataId { get; set; }
        public double CarbohydratesIntakeCount { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
    }
}
