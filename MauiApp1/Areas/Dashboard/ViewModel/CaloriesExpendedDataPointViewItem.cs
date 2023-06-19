using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceIntegration.Common.Data
{
    public class CaloriesExpendedDataPointViewItem
    {
        public int? CaloriesExpended { get; set; }

        public DateTime UtcStartDateTime { get; set; }

        public DateTime UtcEndDateTime { get; set; }

        public DateTimeOffset LocalStartDateTimeOffset { get; set; }

        public DateTimeOffset LocalEndDateTimeOffset { get; set; }
    }
}
