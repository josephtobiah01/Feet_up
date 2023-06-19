using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Areas.Dashboard.ViewModel
{
    public class FatIntakeViewItem
    {
        public Guid FatDataId { get; set; }
        public long FatIntakeCount { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
    }
}
