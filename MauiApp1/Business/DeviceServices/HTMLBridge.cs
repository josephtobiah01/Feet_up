using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Business.DeviceServices
{
    public static class HTMLBridge
    {
        public static double  DeviceWidth { get; set; }
        public static double  DeviceHeight { get; set; }

        public static EventHandler RefreshData;

        public static EventHandler RefreshMenu;

        public static StackLayout MainPageBlackStackLayout { get; set; }

        public static ActivityIndicator MainPageLoadingActivityIndicator { get; set; }

        public static double BrowserInnerHeight { get; set; }
        public static double BrowserInnerWidth { get; set; }

    }
}
