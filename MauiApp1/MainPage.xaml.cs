
using DevExpress.Maui.Controls;
using MauiApp1._Push;
using MauiApp1.Business.DeviceServices;
using MauiApp1.EventArguments;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using ParentMiddleWare;
using ParentMiddleWare.Models.DeviceIntegration;

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

           
            InitializeErrorBottomSheetManager();
        }


        private void ContentPage_Loaded(object sender, EventArgs e)
        {
            InitializeConnectivityManagerBottomSheet();
            CheckInternetConnection();
            CheckStorageReadError();
            InitializeDeviceInformation();
        }

        private void ContentPage_Unloaded(object sender, EventArgs e)
        {
           
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
            if (HTMLBridge.TemplateErrorPopup != null)
            {
                HTMLBridge.TemplateErrorPopup = this.TemplateErrorPopup;
            }
            else
            {
                HTMLBridge.TemplateErrorPopup = null;
                HTMLBridge.TemplateErrorPopup = this.TemplateErrorPopup;
            }
            if (HTMLBridge.TemplateErrorHeader != null)
            {
                HTMLBridge.TemplateErrorHeader = this.TemplateErrorHeader;
            }
            else
            {
                HTMLBridge.TemplateErrorHeader = null;
                HTMLBridge.TemplateErrorHeader = this.TemplateErrorHeader;
            }
            if (HTMLBridge.TemplateErrorLabel != null)
            {
                HTMLBridge.TemplateErrorLabel = this.TemplateErrorLabel;
            }
            else
            {
                HTMLBridge.TemplateErrorLabel = null;
                HTMLBridge.TemplateErrorLabel = this.TemplateErrorLabel;
            }
            if (HTMLBridge.LostInternetPopup != null)
            {
                HTMLBridge.LostInternetPopup = this.LostInternetBottomSheet;
            }
            else
            {
                HTMLBridge.LostInternetPopup = null;
                HTMLBridge.LostInternetPopup = this.LostInternetBottomSheet;
            }
#endif
            App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);


        }

        private void InitializePopups()
        {
            System.Diagnostics.Debug.WriteLine(DeviceDisplay.Current.MainDisplayInfo.Width);
            if (DeviceDisplay.Current.MainDisplayInfo.Width <= 400)
            {
                NotificationPermissionPopup.HalfExpandedRatio = 0.8;
                CameraPermissionPopup.HalfExpandedRatio = 0.8;
                StorageReadWritePermissionPopup.HalfExpandedRatio = 0.8;
                LostInternetBottomSheet.HalfExpandedRatio = 0.9;
                NotificationPermissionPopup.HalfExpandedRatio = 0.9;
                TemplateErrorPopup.HalfExpandedRatio = 0.9;
            }
        }

        private void InitializeConnectivityManagerBottomSheet()
        {
            if (App.connectivityManager != null)
            {
                if(App.connectivityManager.OpenNoInternetConnectionBottomSheetHasSubscription() == false)
                {
                    App.connectivityManager.OpenNoInternetConnectionBottomSheet -= ConnectivityManager_ShowNoInternetErrorBottomSheet;
                    App.connectivityManager.OpenNoInternetConnectionBottomSheet += ConnectivityManager_ShowNoInternetErrorBottomSheet;
                }                
            }
        }        

        private void InitializeErrorBottomSheetManager()
        {
            if (App.alertBottomSheetManager != null)
            {
                App.alertBottomSheetManager.ClearEventSubscriptionOpenConfirmationBottomSheet();
                App.alertBottomSheetManager.ClearEventSubscriptionOpenInternalServerErrorBottomSheet();
                App.alertBottomSheetManager.ClearEventSubscriptionOpenAlertBottomSheet();

                //App.alertBottomSheetManager.OpenAlertBottomSheet -= AlertBottomSheetManager_ShowErrorBottomSheet;
                App.alertBottomSheetManager.OpenAlertBottomSheet += AlertBottomSheetManager_ShowErrorBottomSheet;

                //App.alertBottomSheetManager.OpenInternalServerErrorBottomSheet -= AlertBottomSheetManager_ShowInternalServerErrorBottomSheet;                
                App.alertBottomSheetManager.OpenInternalServerErrorBottomSheet += AlertBottomSheetManager_ShowInternalServerErrorBottomSheet;

                //App.alertBottomSheetManager.OpenConfirmationBottomSheet -= AlertBottomSheetManager_ShowConfirmationBottomSheet;
                App.alertBottomSheetManager.OpenConfirmationBottomSheet += AlertBottomSheetManager_ShowConfirmationBottomSheet;

            }
        }      

        private async void InitializeDeviceInformation()
        {
            try
            {
                if (App.deviceInformationManager.DeviceId == Guid.Empty)
                {
                    App.deviceInformationManager.DeviceId = await DeviceStorageManager.GetDeviceIdStorageData();
                }                   

                if(App.deviceInformationManager.DeviceId == Guid.Empty)
                {
                    App.deviceInformationManager.CreateDeviceId();
                    DeviceStorageManager.SaveDeviceIdData(App.deviceInformationManager.DeviceId);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {

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

        private void CheckStorageReadError()
        {
            if(DeviceStorageManager.HasErrorReadingStorage == true)
            {
                OpenTemplatePopup("Retrieve Device Data","Error occurred when reading file storage.","OK");
                DeviceStorageManager.HasErrorReadingStorage = false;
            }                
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

        public void OpenLostInternetPopup()
        {
            this.LostInternetBottomSheet.State = BottomSheetState.HalfExpanded;
        }

        public void CloseLostInternetPopup(object sender, EventArgs e)
        {
            this.LostInternetBottomSheet.State = BottomSheetState.Hidden;
        }

        public void CloseTemplatePopup(object sender, EventArgs e)
        {
            this.TemplateErrorPopup.State = BottomSheetState.Hidden;
        }

        public void OpenTemplatePopup(string header, string label, string cancelMessage)
        {
            this.TemplateErrorHeader.Text = header;
            this.TemplateErrorLabel.Text = label;
            this.TemplateErrorButton.Text = cancelMessage;
            this.TemplateErrorPopup.State = BottomSheetState.HalfExpanded;
        }

        public void OpenConfirmationBottomSheed(string title, string message, string acceptMessage, string cancelMessage)
        {
            this.CustomComfirmationTitle.Text = title;
            this.CustomComfirmationMessage.Text = message;
            this.CustomComfirmationAcceptButton.Text = acceptMessage;
            this.CustomComfirmationCancelButton.Text = cancelMessage;
            this.CustomComfirmationBottomSheet.State = BottomSheetState.HalfExpanded;
        }


        #endregion

        #region [Methods :: EventHandlers :: Controls]

        private void GoToSettingBottomSheetButton_Clicked(object sender, EventArgs e)
        {
            this.LostInternetBottomSheet.State = BottomSheetState.Hidden;
            GoToSettingApplication();
        }

        private void ConnectivityManager_ShowNoInternetErrorBottomSheet(object sender, EventArgs eventArgs)
        {
            ShowNoInternetErrorBottomSheet();
        }

        private void AlertBottomSheetManager_ShowErrorBottomSheet(object sender, AlertMessageEventArgs alertMessageEventArgs)
        {
            OpenTemplatePopup(alertMessageEventArgs.Title, alertMessageEventArgs.Message, alertMessageEventArgs.CancelMessage);
        }
        
        private void AlertBottomSheetManager_ShowConfirmationBottomSheet(object sender, AlertMessageEventArgs alertMessageEventArgs)
        {
            OpenConfirmationBottomSheed(alertMessageEventArgs.Title, alertMessageEventArgs.Message,alertMessageEventArgs.AcceptMessage, alertMessageEventArgs.CancelMessage);
        }

        private void AlertBottomSheetManager_ShowInternalServerErrorBottomSheet(object sender, EventArgs eventArgs)
        {
            ShowInternalServerErrorBottomSheet();
        }

        private void GoBackInternalServerErrorBottomSheetButton_Clicked(object sender, EventArgs e)
        {
            HideInternalServerErrorBottomSheet();
        }

        private void CloseInternalServerErrorBottomSheetButton_Clicked(object sender, EventArgs e)
        {
            HideInternalServerErrorBottomSheet();
        }

        private void CustomComfirmationCloseButton_Clicked(object sender, EventArgs e)
        {
            HideConfirmationBottomSheet();
        }

        private void CustomComfirmationAcceptButton_Clicked(object sender, EventArgs e)
        {
            HandleConfirmAcceptClick();
        }

        private void CustomComfirmationCancelButton_Clicked(object sender, EventArgs e)
        {
            HandleConfirmCancelClick();
        }
        #endregion

        #region [Methods :: Tasks]        

        private void GoToSettingApplication()
        {
            AppInfo.Current.ShowSettingsUI();
        }

        private void ShowNoInternetErrorBottomSheet()
        {
            this.LostInternetBottomSheet.State = BottomSheetState.HalfExpanded;
        }

        private void CheckInternetConnection()
        {
            if(App.connectivityManager != null)
            {
                App.connectivityManager.CheckInternetConnection();
            }
        }

        private void ShowInternalServerErrorBottomSheet()
        {
            this.InternalErrorBottomSheet.State = BottomSheetState.HalfExpanded;
        }

        private void HideInternalServerErrorBottomSheet()
        {
            this.InternalErrorBottomSheet.State = BottomSheetState.Hidden;
        }

        private void HideConfirmationBottomSheet()
        {
            this.CustomComfirmationBottomSheet.State = BottomSheetState.Hidden;
        }

        private void HandleConfirmAcceptClick()
        {
            App.alertBottomSheetManager.AcceptConfirmationBottomSheet_Invoke();
            HideConfirmationBottomSheet();
        }

        private void HandleConfirmCancelClick()
        {
            App.alertBottomSheetManager.CancelConfirmationBottomSheet_Invoke();
            HideConfirmationBottomSheet();
        }

        #endregion

      
    }
}