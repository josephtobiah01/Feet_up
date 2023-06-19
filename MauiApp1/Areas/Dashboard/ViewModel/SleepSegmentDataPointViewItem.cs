using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceIntegration.Common.Data
{
    public class SleepSegmentDataPointViewItem
    {
        public double Duration { get; set; }
        public int? SleepStageTypeId { get; set; }

        public string? SleepStageType { get; set; }

        public DateTime UtcDateTime { get; set; }

        public DateTimeOffset LocalDateTimeOffset { get; set; }
    }
}
