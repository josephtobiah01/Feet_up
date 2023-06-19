using DeviceIntegration.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceIntegration.Client.Business
{
    public class GoogleFitStepsCountDataManager
    {
        public List<StepsCountDataPointViewItem> GetLatestDailyStepsCountDataByDateRange(DateTimeOffset startDateTimeOffset, DateTimeOffset endDateTimeOffset)
        {
            List<StepsCountDataPointViewItem> stepsCountDataPointViewItems = new List<StepsCountDataPointViewItem>();

            StepsCountDataPointViewItem stepsCountDataPointViewItem = null;

            stepsCountDataPointViewItem = new StepsCountDataPointViewItem();
            stepsCountDataPointViewItem.Steps = 8701;
            stepsCountDataPointViewItem.LocalStartDateTimeOffset = new DateTimeOffset(2023, 06, 12, 0, 45, 0, TimeSpan.FromHours(8));
            stepsCountDataPointViewItem.LocalEndDateTimeOffset = new DateTimeOffset(2023, 06, 12, 23, 45, 0, TimeSpan.FromHours(8));
            stepsCountDataPointViewItems.Add(stepsCountDataPointViewItem);

            stepsCountDataPointViewItem = new StepsCountDataPointViewItem();
            stepsCountDataPointViewItem.Steps = 5838;
            stepsCountDataPointViewItem.LocalStartDateTimeOffset = new DateTimeOffset(2023, 06, 13, 0, 45, 0, TimeSpan.FromHours(8));
            stepsCountDataPointViewItem.LocalEndDateTimeOffset = new DateTimeOffset(2023, 06, 13, 23, 45, 0, TimeSpan.FromHours(8));
            stepsCountDataPointViewItems.Add(stepsCountDataPointViewItem);

            stepsCountDataPointViewItem = new StepsCountDataPointViewItem();
            stepsCountDataPointViewItem.Steps = 80;
            stepsCountDataPointViewItem.LocalStartDateTimeOffset = new DateTimeOffset(2023, 06, 15, 0, 45, 0, TimeSpan.FromHours(8));
            stepsCountDataPointViewItem.LocalEndDateTimeOffset = new DateTimeOffset(2023, 06, 15, 23, 45, 0, TimeSpan.FromHours(8));
            stepsCountDataPointViewItems.Add(stepsCountDataPointViewItem);

            stepsCountDataPointViewItems = stepsCountDataPointViewItems.Where(t => t.LocalStartDateTimeOffset >= startDateTimeOffset && t.LocalEndDateTimeOffset <= endDateTimeOffset).ToList();

            return stepsCountDataPointViewItems;
        }
    }
}
