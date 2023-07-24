using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !WINDOWS
using DevExpress.Maui.Controls;
using DevExpress.Maui.Editors;
#endif

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
#if !WINDOWS
        public static DXPopup DXCalenderPopup { get; set; }
        public static BottomSheet CameraPermissionPopup { get; set; } 
        public static BottomSheet StoragePermissionPopup { get; set; } 
        public static BottomSheet NotificationPermissionPopup { get; set; } 
#endif

        public static double BrowserInnerHeight { get; set; }
        public static double BrowserInnerWidth { get; set; }

    }
}
