using DeviceIntegration.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceIntegration.Client.Business
{
    public class GoogleFitCaloriesExpendedDataManager
    {
        public List<CaloriesExpendedDataPointViewItem> GetLatestDailyCaloriesExpendedDataByDateRange(DateTimeOffset startDateTimeOffset, DateTimeOffset endDateTimeOffset)
        {
            List<CaloriesExpendedDataPointViewItem> caloriesExpendedDataPointViewItems = new List<CaloriesExpendedDataPointViewItem>();

            CaloriesExpendedDataPointViewItem caloriesExpendedDataPointViewItem = null;

            caloriesExpendedDataPointViewItem = new CaloriesExpendedDataPointViewItem();
            caloriesExpendedDataPointViewItem.CaloriesExpended = 500;
            //caloriesExpendedDataPointViewItem.CaloriesExpendedTypeId = 1;
            caloriesExpendedDataPointViewItem.LocalStartDateTimeOffset = new DateTimeOffset(2023, 06, 12, 0, 45, 0, TimeSpan.FromHours(8));
            caloriesExpendedDataPointViewItem.LocalEndDateTimeOffset = new DateTimeOffset(2023, 06, 12, 23, 45, 0, TimeSpan.FromHours(8));
            caloriesExpendedDataPointViewItems.Add(caloriesExpendedDataPointViewItem);

            caloriesExpendedDataPointViewItem = new CaloriesExpendedDataPointViewItem();
            caloriesExpendedDataPointViewItem.CaloriesExpended = 1908;
            //caloriesExpendedDataPointViewItem.CaloriesExpendedTypeId = 2;
            caloriesExpendedDataPointViewItem.LocalStartDateTimeOffset = new DateTimeOffset(2023, 06, 12, 0, 45, 0, TimeSpan.FromHours(8));
            caloriesExpendedDataPointViewItem.LocalEndDateTimeOffset = new DateTimeOffset(2023, 06, 12, 23, 45, 0, TimeSpan.FromHours(8));
            caloriesExpendedDataPointViewItems.Add(caloriesExpendedDataPointViewItem);


            caloriesExpendedDataPointViewItem = new CaloriesExpendedDataPointViewItem();
            caloriesExpendedDataPointViewItem.CaloriesExpended = 600;
            //caloriesExpendedDataPointViewItem.CaloriesExpendedTypeId = 1;
            caloriesExpendedDataPointViewItem.LocalStartDateTimeOffset = new DateTimeOffset(2023, 06, 13, 0, 45, 0, TimeSpan.FromHours(8));
            caloriesExpendedDataPointViewItem.LocalEndDateTimeOffset = new DateTimeOffset(2023, 06, 13, 23, 45, 0, TimeSpan.FromHours(8));
            caloriesExpendedDataPointViewItems.Add(caloriesExpendedDataPointViewItem);

            caloriesExpendedDataPointViewItem = new CaloriesExpendedDataPointViewItem();
            caloriesExpendedDataPointViewItem.CaloriesExpended = 2291;
            //caloriesExpendedDataPointViewItem.CaloriesExpendedTypeId = 2;
            caloriesExpendedDataPointViewItem.LocalStartDateTimeOffset = new DateTimeOffset(2023, 06, 13, 0, 45, 0, TimeSpan.FromHours(8));
            caloriesExpendedDataPointViewItem.LocalEndDateTimeOffset = new DateTimeOffset(2023, 06, 13, 23, 45, 0, TimeSpan.FromHours(8));
            caloriesExpendedDataPointViewItems.Add(caloriesExpendedDataPointViewItem);



            caloriesExpendedDataPointViewItem = new CaloriesExpendedDataPointViewItem();
            caloriesExpendedDataPointViewItem.CaloriesExpended = 800;
            //caloriesExpendedDataPointViewItem.CaloriesExpendedTypeId = 1;
            caloriesExpendedDataPointViewItem.LocalStartDateTimeOffset = new DateTimeOffset(2023, 06, 15, 0, 45, 0, TimeSpan.FromHours(8));
            caloriesExpendedDataPointViewItem.LocalEndDateTimeOffset = new DateTimeOffset(2023, 06, 15, 23, 45, 0, TimeSpan.FromHours(8));
            caloriesExpendedDataPointViewItems.Add(caloriesExpendedDataPointViewItem);

            caloriesExpendedDataPointViewItem = new CaloriesExpendedDataPointViewItem();
            caloriesExpendedDataPointViewItem.CaloriesExpended = 1561;
            //caloriesExpendedDataPointViewItem.CaloriesExpendedTypeId = 2;
            caloriesExpendedDataPointViewItem.LocalStartDateTimeOffset = new DateTimeOffset(2023, 06, 15, 0, 45, 0, TimeSpan.FromHours(8));
            caloriesExpendedDataPointViewItem.LocalEndDateTimeOffset = new DateTimeOffset(2023, 06, 15, 23, 45, 0, TimeSpan.FromHours(8));
            caloriesExpendedDataPointViewItems.Add(caloriesExpendedDataPointViewItem);

            caloriesExpendedDataPointViewItems = caloriesExpendedDataPointViewItems.Where(t => t.LocalStartDateTimeOffset >= startDateTimeOffset && t.LocalEndDateTimeOffset <= endDateTimeOffset).ToList();

            return caloriesExpendedDataPointViewItems;
        }
    }
}
