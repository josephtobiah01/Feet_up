using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Areas.Dashboard.ViewModel
{
    public class ProteinIntakeViewItem
    {
        public Guid ProteinDataId { get; set; }
        public long ProteinIntakeCount { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
    }
}
