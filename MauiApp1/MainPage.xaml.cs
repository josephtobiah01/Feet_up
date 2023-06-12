
using MauiApp1.Business.DeviceServices;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

#if ANDROID
// Needed for Picking photo/video
[assembly: Android.App.UsesPermission(Android.Manifest.Permission.ReadExternalStorage, MaxSdkVersion = 32)]
[assembly: Android.App.UsesPermission(Android.Manifest.Permission.ReadMediaAudio)]
[assembly: Android.App.UsesPermission(Android.Manifest.Permission.ReadMediaImages)]
[assembly: Android.App.UsesPermission(Android.Manifest.Permission.ReadMediaVideo)]

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
        }

        private void InitializeData()
        {
            HTMLBridge.DeviceHeight = DeviceDisplay.Current.MainDisplayInfo.Height;
            HTMLBridge.DeviceHeight = DeviceDisplay.Current.MainDisplayInfo.Width;

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
            App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
           // Preferences.Default.Set("TestMode", true);

        }

        #endregion


    }
}