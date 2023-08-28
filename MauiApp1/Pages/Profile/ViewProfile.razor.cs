using FitnessData.Common;
using MauiApp1.Areas.BarcodeScanning.Views;
using MauiApp1.Areas.Security.Views;
using MauiApp1.Business;
using MauiApp1.Business.DeviceServices;
using MauiApp1.Business.UserServices;
using MauiApp1.Exceptions;
using Microsoft.AspNetCore.Components;
using Microsoft.Maui.Authentication;
using Newtonsoft.Json;
using ParentMiddleWare;
using ParentMiddleWare.Models.DeviceIntegration;
using UserApi.Net7;

namespace MauiApp1.Pages.Profile
{
    //
    // Summary:
    //     Web Authenticator Result parsed from the callback Url.
    //
    // Remarks:
    //     All of the querystring or url fragment properties are parsed into a dictionary
    //     and can be accessed by their key.

    public class WebAuthenticatorResultProxy
    {
        [JsonConstructor]
        public WebAuthenticatorResultProxy()
        {

        }

        public Dictionary<string, string> Properties { get; set; }


    }


    public partial class ViewProfile
    {
        #region [Fields]
        [Parameter]
        public EventCallback OpenCameraPermissionCallback { get; set; }
        private bool _hideConnectedAppButton = true;
        #endregion

        #region[Initialization]

        protected override async void OnInitialized()
        {
            base.OnInitialized();
            IntializeConfirmationBottomSheetAction();

            await RefreshMenu();
        }

        private void IntializeConfirmationBottomSheetAction()
        {
            if (App.alertBottomSheetManager != null)
            {
                App.alertBottomSheetManager.AcceptButtonConfirmationBottomSheet -= CustomComfirmationAcceptlButton_Clicked;
                App.alertBottomSheetManager.AcceptButtonConfirmationBottomSheet += CustomComfirmationAcceptlButton_Clicked;

                App.alertBottomSheetManager.CancelButtonConfirmationBottomSheet -= CustomComfirmationCancelButton_Clicked;
                App.alertBottomSheetManager.CancelButtonConfirmationBottomSheet += CustomComfirmationCancelButton_Clicked;

            }
        }


        #endregion

        #region [Methods :: EventHandlers :: Controls]

        private async Task GoToScanPage()
        {
            bool IsCameraAllowed = await CheckCameraPermissions();
            if (IsCameraAllowed)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new BarcodeScannerContentPage());
            }
            else
            {
                await OpenCameraPermissionCallback.InvokeAsync();
            }
        }

        public static async Task<bool> CheckCameraPermissions()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status == PermissionStatus.Denied || status == PermissionStatus.Unknown || status == PermissionStatus.Disabled)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public async void ClosePage()
        {
            await App.Current.MainPage.Navigation.PopAsync();
        }

        private void ConnectedApps_Clicked()
        {
            HandleConnectApplicationClick();
        }

        private void LogOutButton_Clicked()
        {
            ShowConfirmationBottomSheet("Sign Out", "Are you sure you want to sign out", "Sign out", "Cancel");
        }

        private async void CustomComfirmationAcceptlButton_Clicked(object sender, EventArgs e)
        {
            await SignOutUser(true);
        }

        private async void CustomComfirmationCancelButton_Clicked(object sender, EventArgs e)
        {
            await SignOutUser(false);
        }

        #endregion

        #region [Methods :: Tasks]

        private async Task RefreshMenu()
        {
            bool isAppConnected = false;
            bool userSignedIn = MiddleWare.FkFederatedUser != string.Empty;
            try
            {
                await Task.Run(async () =>
                {
                    if (userSignedIn == true)
                    {
                        if (ConnectedDeviceDataStorageManager.googleFit != null)
                        {

                            //Check if google fit access token is already connected
                            ConnectApplicationDeviceManager.mobileUserAccountAdministrator =
                                              new SecurityServices.Client.Business.MobileUserAccountAdministrator(
                                                  MiddleWare.AppConnectEndpoint,
                                                  ParentMiddleWare.ConnectedDeviceDataStorageManager.googleFit.AccessToken);

                            isAppConnected = ConnectApplicationDeviceManager.mobileUserAccountAdministrator
                                .IsAlreadyConnected(MiddleWare.FkFederatedUser);

                            if (isAppConnected == true &&
                                string.IsNullOrWhiteSpace(
                                    ParentMiddleWare.ConnectedDeviceDataStorageManager.googleFit.AccessToken) == false)
                            {
                                _hideConnectedAppButton = true;
                            }
                            else
                            {
                                _hideConnectedAppButton = false;
                            }
                        }
                        else if (ConnectedDeviceDataStorageManager.appleHealthKit != null)
                        {
                            //Check if apple health kit access token is already connected
                        }
                        else
                        {
                            // none
                            _hideConnectedAppButton = false;
                        }

                    }
                    else
                    {
                        _hideConnectedAppButton = true;
                    }

                    await InvokeAsync(() =>
                    {
                        StateHasChanged();
                    });
                });
            }
            catch (DeviceStorageException deviceStorageException)
            {
                await ForceSignOutUser();
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }

        }

        private async void HandleConnectApplicationClick()
        {

            try
            {
                if (ConnectedDeviceDataStorageManager.googleFit == null)
                {
                    await ConnectToGoogleFit();
                }
                else
                {
                    ConnectApplicationDeviceManager.mobileUserAccountAdministrator =
                                       new SecurityServices.Client.Business.MobileUserAccountAdministrator(
                                           MiddleWare.AppConnectEndpoint,
                                           ParentMiddleWare.ConnectedDeviceDataStorageManager.googleFit.AccessToken);

                    bool isAppConnected = ConnectApplicationDeviceManager.mobileUserAccountAdministrator.IsAlreadyConnected(MiddleWare.FkFederatedUser);
                    if (isAppConnected == false
                        || string.IsNullOrWhiteSpace(ParentMiddleWare.ConnectedDeviceDataStorageManager.googleFit.AccessToken) == true)
                    {
                        await ConnectToGoogleFit();
                    }
                    else
                    {
                        //await App.Current.MainPage.DisplayAlert("Notification", "You are already connected with Age In Reverse.", "OK");
                        ShowAlertBottomSheet("Notification", "You are already connected with Age In Reverse.", "OK");
                    }
                }

            }
            catch (DeviceStorageException deviceStorageException)
            {
                await ForceSignOutUser();
            }
            catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException)
            {
                //await App.Current.MainPage.DisplayAlert("Connect App", "Connect to Google Fit was cancelled.", "OK");
                ShowAlertBottomSheet("Connect App", "Connect to Google Fit was cancelled.", "OK");
            }
            catch (Exception ex)
            {
                //await App.Current.MainPage.DisplayAlert("Connect to Google Fit", ex.Message + ex.StackTrace, "OK");
                ShowAlertBottomSheet("Connect to Google Fit", "An error occured while connecting to google fit. Please check your connection and try again.", "OK");
            }
            finally
            {

            }

        }

        private async Task ConnectToGoogleFit()
        {
            string expiresInString = string.Empty;
            string refreshTokenExpiresInString = string.Empty;          

            try
            {
#if ANDROID || IOS

                WebAuthenticatorResult webAuthenticatorResult =
                    await WebAuthenticator.Default.AuthenticateAsync(new Uri(
                        MiddleWare.GoogleMobileAuthenticationEndpoint), new Uri("myapp://")
                );

                if (webAuthenticatorResult != null)
                {
                    if (ParentMiddleWare.ConnectedDeviceDataStorageManager.googleFit == null)
                    {
                        ParentMiddleWare.ConnectedDeviceDataStorageManager.googleFit = new ParentMiddleWare.Models.DeviceIntegration.GoogleFit();
                    }

                    DeviceStorageManager.SaveGoogleFitData(webAuthenticatorResult);

                    ConnectApplicationDeviceManager.mobileUserAccountAdministrator =
                                      new SecurityServices.Client.Business.MobileUserAccountAdministrator(
                                          MiddleWare.AppConnectEndpoint,
                                          ParentMiddleWare.ConnectedDeviceDataStorageManager.googleFit.AccessToken);

                    ConnectApplicationDeviceManager.mobileUserAccountAdministrator.ConnectGoogleAccount(
                        MiddleWare.FkFederatedUser, App.deviceInformationManager.DeviceId
                        , MiddleWare.UserName, MiddleWare.UserName,
                        ParentMiddleWare.ConnectedDeviceDataStorageManager.googleFit.EmailAddress, string.Empty,
                            ParentMiddleWare.ConnectedDeviceDataStorageManager.googleFit.AccessToken,
                            ParentMiddleWare.ConnectedDeviceDataStorageManager.googleFit.RefreshToken,
                                ParentMiddleWare.ConnectedDeviceDataStorageManager.googleFit.ExpiresIn,
                                ParentMiddleWare.ConnectedDeviceDataStorageManager.googleFit.RefreshExpiresIn);

                    RefreshMenu();
                    StateHasChanged();

                    //await App.Current.MainPage.DisplayAlert("Google Fit Connected Successfully", "It may take some time for apps and devices to sync data with Google Fit.", "OK");
                    ShowAlertBottomSheet("Google Fit Connected Successfully", "It may take some time for apps and devices to sync data with Google Fit.", "OK");
                }
                else
                {
                    //await App.Current.MainPage.DisplayAlert("Unable to Connect Google Fit", "", "OK");
                    ShowAlertBottomSheet("Unable to Connect Google Fit", "", "OK");
                }
#endif
            }
            catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException)
            {
                throw;
                //ShowAlertBottomSheet("Connect App", "Connect to Google Fit was cancelled.", "OK");
            }

        }

        private async Task ForceSignOutUser()
        {
            await DeviceUserAccountManager.ForceUserSignOut();
            if (HTMLBridge.RefreshData != null)
            {
                HTMLBridge.RefreshData.Invoke(this, null);
            }
            if (App.Current.MainPage != null)
            {
                //await App.Current.MainPage.Navigation.PushAsync(new MVPLoginContentPage());
                App.Current.MainPage = new NavigationPage(new MVPLoginContentPage());
                await Application.Current.MainPage.Navigation.PopToRootAsync();
            }
        }

        //private async Task SignOutUser()
        //{

        //    bool userSignedOut = await App.Current.MainPage.DisplayAlert("Sign Out", "Are you sure you want to sign out", "Sign out", "Cancel");

        //    if (userSignedOut == true)
        //    {
        //        string FkFederatedUser = MiddleWare.FkFederatedUser;
        //        bool userSignedOutSuccessful = UserHandler.LogOffuser();

        //        if (userSignedOutSuccessful == true)
        //        {
        //            try
        //            {
        //                await UserMiddleware.UnRegisterDevice(await PushRegistration.CheckPermission(), FkFederatedUser, PushRegistration.GetPlatform());
        //            }
        //            catch (Exception ex)
        //            {

        //            }
        //            if (HTMLBridge.RefreshData != null)
        //            {
        //                HTMLBridge.RefreshData.Invoke(this, null);
        //            }
        //            await App.Current.MainPage.Navigation.PushAsync(new MVPLoginContentPage());
        //        }

        //        DeviceStorageManager.ClearFitnessServiceStorage(MiddleWare.UseSecuredStorage);
        //        StateHasChanged();
        //    }
        //    else
        //    {
        //        StateHasChanged();
        //    }
        //}

        private async Task SignOutUser(bool userSignedOut)
        {

            //bool userSignedOut = await App.Current.MainPage.DisplayAlert("Sign Out", "Are you sure you want to sign out", "Sign out", "Cancel");

            if (userSignedOut == true)
            {
                string FkFederatedUser = MiddleWare.FkFederatedUser;
                bool userSignedOutSuccessful = UserHandler.LogOffuser();

                if (userSignedOutSuccessful == true)
                {
                    try
                    {
                        await UserMiddleware.UnRegisterDevice(await PushRegistration.CheckPermission(), FkFederatedUser, PushRegistration.GetPlatform());
                    }
                    catch (Exception ex)
                    {

                    }
                    if (HTMLBridge.RefreshData != null)
                    {
                        HTMLBridge.RefreshData.Invoke(this, null);
                    }
                    //await App.Current.MainPage.Navigation.PushAsync(new MVPLoginContentPage());
                    App.Current.MainPage = new NavigationPage(new MVPLoginContentPage());
                    await Application.Current.MainPage.Navigation.PopToRootAsync();
                }

                DeviceStorageManager.ClearFitnessServiceStorage(MiddleWare.UseSecuredStorage);
                StateHasChanged();
            }
            else
            {
                StateHasChanged();
            }
        }

        private void ShowConfirmationBottomSheet(string title, string message, string acceptMessage, string cancelMessage)
        {
            if (App.alertBottomSheetManager != null)
            {
                App.alertBottomSheetManager.ShowConfirmationMessage(title, message, acceptMessage, cancelMessage);
            }
        }

        private void ShowAlertBottomSheet(string title, string message, string cancelMessage)
        {
            if (App.alertBottomSheetManager != null)
            {
                App.alertBottomSheetManager.ShowAlertMessage(title, message, cancelMessage);
            }
        }
        #endregion

    }
}