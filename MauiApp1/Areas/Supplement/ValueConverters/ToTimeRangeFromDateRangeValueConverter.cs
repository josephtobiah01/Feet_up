using FeedApi.Net7.Models;
using MauiApp1.Areas.Supplement.ViewModels;
using ParentMiddleWare;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Areas.Supplement.ValueConverters
{
    public class ToTimeRangeFromDateRangeValueConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string timeRange = string.Empty;
            //if(value is SupplementPageViewModel)
            //{
            //    timeRange = GetFormattedFromToTime((value as SupplementPageViewModel).FromDate, (value as SupplementPageViewModel).ToDate);
            //}

            return timeRange;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string timeRange = string.Empty;
            //if (value is SupplementPageViewModel)
            //{
            //    timeRange = GetFormattedFromToTime((value as SupplementPageViewModel).FromDate, (value as SupplementPageViewModel).ToDate);
            //}

            return timeRange;
        }

        private string GetFormattedFromToTime(DateTime fromDateTime, DateTime toDateTime)
        {
            string elapsed = string.Empty;

            string fromTime = fromDateTime.ToString("hh:mm");
            string toTime = toDateTime.ToString("hh:mm");

            elapsed = string.Format("{0} to {1}", fromTime, toTime);

            return elapsed;
        }

    }
}
