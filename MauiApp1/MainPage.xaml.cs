
using DevExpress.Maui.Controls;
using MauiApp1._Push;
using MauiApp1.Business.DeviceServices;
using Microsoft.AspNetCore.Components;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

#if ANDROID
// Needed for Picking photo/video
[assembly: Android.App.UsesPermission(Android.Manifest.Permission.ReadExternalStorage, MaxSdkVersion = 32)]
[assembly: Android.App.UsesPermission(Android.Manifest.Permission.ReadMediaAudio)]
[assembly: Android.App.UsesPermission(Android.Manifest.Permission.ReadMediaImages)]
[assembly: Android.App.UsesPermission(Android.Manifest.Permission.ReadMediaVideo)]
[assembly: Android.App.UsesPermission(Android.Manifest.Permission.PostNotifications)]
[assembly: Android.App.UsesPermission(Android.Manifest.Permission.AccessNotificationPolicy)]
[assembly: Android.App.UsesPermission(Android.Manifest.Permission.SystemAlertWindow)]


// Needed for Taking photo/video
[assembly: Android.App.UsesPermission(Android.Manifest.Permission.Camera)]
[assembly: Android.App.UsesPermission(Android.Manifest.Permission.WriteExternalStorage, MaxSdkVersion = 32)]

// Add these properties if you would like to filter out devices that do not have cameras, or set to false to make them optional
[assembly: Android.App.UsesFeature("android.hardware.camera", Required = true)]
[assembly: Android.App.UsesFeature("android.hardware.camera.autofocus", Required = true)]
#endif

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        #region [Methods :: EventHandlers :: Class]

        public MainPage()
        {

            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            InitializeData();
            InitializePopups();
        }

        private void InitializeData()
        {
            HTMLBridge.DeviceHeight = DeviceDisplay.Current.MainDisplayInfo.Height;
            HTMLBridge.DeviceWidth = DeviceDisplay.Current.MainDisplayInfo.Width;

            if (HTMLBridge.RefreshData != null)
            {
                HTMLBridge.RefreshData.Invoke(this, null);
            }

            if (HTMLBridge.MainPageBlackStackLayout != null)
            {
                HTMLBridge.MainPageBlackStackLayout = this.BlackStackLayout;
            }
            else
            {
                HTMLBridge.MainPageBlackStackLayout = null;
                HTMLBridge.MainPageBlackStackLayout = this.BlackStackLayout;
            }


            if (HTMLBridge.MainPageLoadingActivityIndicator != null)
            {
                HTMLBridge.MainPageLoadingActivityIndicator = this.LoadingActivityIndicator;
            }
            else
            {
                HTMLBridge.MainPageLoadingActivityIndicator = null;
                HTMLBridge.MainPageLoadingActivityIndicator = this.LoadingActivityIndicator;
            }
#if !WINDOWS
            if (HTMLBridge.DXCalenderPopup != null)
            {
                HTMLBridge.DXCalenderPopup = this.CalendarDXPopup;
            }
            else
            {
                HTMLBridge.DXCalenderPopup = null;
                HTMLBridge.DXCalenderPopup = this.CalendarDXPopup;
            }
            if (HTMLBridge.CameraPermissionPopup != null)
            {
                HTMLBridge.CameraPermissionPopup = this.CameraPermissionPopup;
            }
            else
            {
                HTMLBridge.CameraPermissionPopup = null;
                HTMLBridge.CameraPermissionPopup = this.CameraPermissionPopup;
            }
            if (HTMLBridge.StoragePermissionPopup != null)
            {
                HTMLBridge.StoragePermissionPopup = this.StorageReadWritePermissionPopup;
            }
            else
            {
                HTMLBridge.StoragePermissionPopup = null;
                HTMLBridge.StoragePermissionPopup = this.StorageReadWritePermissionPopup;
            }
            if (HTMLBridge.NotificationPermissionPopup != null)
            {
                HTMLBridge.NotificationPermissionPopup = this.NotificationPermissionPopup;
            }
            else
            {
                HTMLBridge.NotificationPermissionPopup = null;
                HTMLBridge.NotificationPermissionPopup = this.NotificationPermissionPopup;
            }
#endif
            App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
           // Preferences.Default.Set("TestMode", true);

        }
        private void InitializePopups()
        {
            System.Diagnostics.Debug.WriteLine(DeviceDisplay.Current.MainDisplayInfo.Width);
            if (DeviceDisplay.Current.MainDisplayInfo.Width <= 400)
            {
                NotificationPermissionPopup.HalfExpandedRatio = 0.8;
                CameraPermissionPopup.HalfExpandedRatio = 0.8;
                StorageReadWritePermissionPopup.HalfExpandedRatio = 0.8;
                LostInternetPopup.HalfExpandedRatio = 0.9;
                NotificationPermissionPopup.HalfExpandedRatio = 0.9;
            }
        }

        public async void DateSelected(object sender, EventArgs e)
        {
#if !WINDOWS
            this.CalendarDXPopup.IsOpen = false;
            if (PushNavigationHelper.RootPage != null)
            {
                await PushNavigationHelper.RootPage.SetDate(this.Calendar.SelectedDate);
            }
#endif
        }
        #region permissions
        public void CloseCameraPermissionPopup(object sender, EventArgs e)
        {
            this.CameraPermissionPopup.State = BottomSheetState.Hidden;
        }
        public void CloseStoragePermissionPopup(object sender, EventArgs e)
        {
            this.StorageReadWritePermissionPopup.State = BottomSheetState.Hidden;
        }
        public void CloseNotificationPermissionPopup(object sender, EventArgs e)
        {
            this.NotificationPermissionPopup.State = BottomSheetState.Hidden;
        }
        public async void RequestCameraPermissions(object sender, EventArgs e)
        {
            if (PushNavigationHelper.RootPage != null)
            {
                await PushNavigationHelper.RootPage.RequestCameraPermissions();
            }
        }
        public async void RequestStoragePermissions(object sender, EventArgs e)
        {
            if (PushNavigationHelper.RootPage != null)
            {
                await PushNavigationHelper.RootPage.RequestFilePermissions();
            }
        }
        public async void RequestNotificationPermissions(object sender, EventArgs e)
        {
            if (PushNavigationHelper.RootPage != null)
            {
                await PushNavigationHelper.RootPage.RequestNotificationPermissions();
            }
        }

        #endregion
        public void CloseLostInternetPopup(object sender, EventArgs e)
        {
            this.LostInternetPopup.State = BottomSheetState.Hidden;
        }
        public void CloseInternalErrorPopup(object sender, EventArgs e)
        {
            this.InternalErrorPopup.State = BottomSheetState.Hidden;
        }


        #endregion


    }
}