using DeviceIntegration.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MauiApp1.Common.DeviceIntegration.DeviceIntegrationEnumerations;

namespace DeviceIntegration.Client.Business
{
    public class GoogleFitSleepSegmentDataManager
    {
        public List<SleepSegmentDataPointViewItem> GetLatestDailySleepSegmentDataByDateRange(DateTimeOffset startDateTimeOffset, DateTimeOffset endDateTimeOffset)
        {
            List<SleepSegmentDataPointViewItem> sleepSegmentDataPointViewItems = new List<SleepSegmentDataPointViewItem>();

            SleepSegmentDataPointViewItem sleepSegmentDataPointViewItem = null;

            sleepSegmentDataPointViewItem = new SleepSegmentDataPointViewItem();
            sleepSegmentDataPointViewItem.Duration = 1;
            sleepSegmentDataPointViewItem.SleepStageTypeId = (int)SleepStageTypes.DeepSleep;
            sleepSegmentDataPointViewItem.LocalDateTimeOffset = new DateTimeOffset(2023, 06, 12, 0, 45, 0, TimeSpan.FromHours(8));
            sleepSegmentDataPointViewItems.Add(sleepSegmentDataPointViewItem);

            sleepSegmentDataPointViewItem = new SleepSegmentDataPointViewItem();
            sleepSegmentDataPointViewItem.Duration = 4;
            sleepSegmentDataPointViewItem.SleepStageTypeId = (int)SleepStageTypes.LightSleep;
            sleepSegmentDataPointViewItem.LocalDateTimeOffset = new DateTimeOffset(2023, 06, 12, 0, 45, 0, TimeSpan.FromHours(8));
            sleepSegmentDataPointViewItems.Add(sleepSegmentDataPointViewItem);

            sleepSegmentDataPointViewItem = new SleepSegmentDataPointViewItem();
            sleepSegmentDataPointViewItem.Duration = 2;
            sleepSegmentDataPointViewItem.SleepStageTypeId = (int)SleepStageTypes.Rem;
            sleepSegmentDataPointViewItem.LocalDateTimeOffset = new DateTimeOffset(2023, 06, 12, 0, 45, 0, TimeSpan.FromHours(8));
            sleepSegmentDataPointViewItems.Add(sleepSegmentDataPointViewItem);

            sleepSegmentDataPointViewItem = new SleepSegmentDataPointViewItem();
            sleepSegmentDataPointViewItem.Duration = 1;
            sleepSegmentDataPointViewItem.SleepStageTypeId = (int)SleepStageTypes.AwakeDuringSleepCycle;
            sleepSegmentDataPointViewItem.LocalDateTimeOffset = new DateTimeOffset(2023, 06, 12, 0, 45, 0, TimeSpan.FromHours(8));
            sleepSegmentDataPointViewItems.Add(sleepSegmentDataPointViewItem);



            sleepSegmentDataPointViewItem = new SleepSegmentDataPointViewItem();
            sleepSegmentDataPointViewItem.Duration = 1;
            sleepSegmentDataPointViewItem.SleepStageTypeId = (int)SleepStageTypes.DeepSleep;
            sleepSegmentDataPointViewItem.LocalDateTimeOffset = new DateTimeOffset(2023, 06, 13, 0, 45, 0, TimeSpan.FromHours(8));
            sleepSegmentDataPointViewItems.Add(sleepSegmentDataPointViewItem);

            sleepSegmentDataPointViewItem = new SleepSegmentDataPointViewItem();
            sleepSegmentDataPointViewItem.Duration = 4.5;
            sleepSegmentDataPointViewItem.SleepStageTypeId = (int)SleepStageTypes.LightSleep;
            sleepSegmentDataPointViewItem.LocalDateTimeOffset = new DateTimeOffset(2023, 06, 13, 0, 45, 0, TimeSpan.FromHours(8));
            sleepSegmentDataPointViewItems.Add(sleepSegmentDataPointViewItem);

            sleepSegmentDataPointViewItem = new SleepSegmentDataPointViewItem();
            sleepSegmentDataPointViewItem.Duration = 2;
            sleepSegmentDataPointViewItem.SleepStageTypeId = (int)SleepStageTypes.Rem;
            sleepSegmentDataPointViewItem.LocalDateTimeOffset = new DateTimeOffset(2023, 06, 13, 0, 45, 0, TimeSpan.FromHours(8));
            sleepSegmentDataPointViewItems.Add(sleepSegmentDataPointViewItem);


            sleepSegmentDataPointViewItem = new SleepSegmentDataPointViewItem();
            sleepSegmentDataPointViewItem.Duration = 1.2;
            sleepSegmentDataPointViewItem.SleepStageTypeId = (int)SleepStageTypes.DeepSleep;
            sleepSegmentDataPointViewItem.LocalDateTimeOffset = new DateTimeOffset(2023, 06, 15, 0, 45, 0, TimeSpan.FromHours(8));
            sleepSegmentDataPointViewItems.Add(sleepSegmentDataPointViewItem);

            sleepSegmentDataPointViewItem = new SleepSegmentDataPointViewItem();
            sleepSegmentDataPointViewItem.Duration = 3;
            sleepSegmentDataPointViewItem.SleepStageTypeId = (int)SleepStageTypes.LightSleep;
            sleepSegmentDataPointViewItem.LocalDateTimeOffset = new DateTimeOffset(2023, 06, 15, 0, 45, 0, TimeSpan.FromHours(8));
            sleepSegmentDataPointViewItems.Add(sleepSegmentDataPointViewItem);

            sleepSegmentDataPointViewItem = new SleepSegmentDataPointViewItem();
            sleepSegmentDataPointViewItem.Duration = 1.5;
            sleepSegmentDataPointViewItem.SleepStageTypeId = (int)SleepStageTypes.Rem;
            sleepSegmentDataPointViewItem.LocalDateTimeOffset = new DateTimeOffset(2023, 06, 15, 0, 45, 0, TimeSpan.FromHours(8));
            sleepSegmentDataPointViewItems.Add(sleepSegmentDataPointViewItem);

            sleepSegmentDataPointViewItem = new SleepSegmentDataPointViewItem();
            sleepSegmentDataPointViewItem.Duration = 0.8;
            sleepSegmentDataPointViewItem.SleepStageTypeId = (int)SleepStageTypes.AwakeDuringSleepCycle;
            sleepSegmentDataPointViewItem.LocalDateTimeOffset = new DateTimeOffset(2023, 06, 15, 0, 45, 0, TimeSpan.FromHours(8));
            sleepSegmentDataPointViewItems.Add(sleepSegmentDataPointViewItem);

            sleepSegmentDataPointViewItems = sleepSegmentDataPointViewItems.Where(t => t.LocalDateTimeOffset.Date >= startDateTimeOffset.Date).ToList();

            return sleepSegmentDataPointViewItems;
        }
    }
}
