using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserApi.Net7;
using CommunityToolkit.Maui.Core.Platform;
using MauiApp1.Pages;
using MauiApp1._Push;
using ParentMiddleWare.ApiModels;
using DevExpress.Maui.Controls;
using MauiApp1.Business.UserServices;
using ParentMiddleWare;
using MauiApp1.Business.DeviceServices;

namespace MauiApp1.Areas.Security.Views;

public partial class MVPLoginContentPage : ContentPage
{
    #region[Fields]

    public bool IsLoginButtonEnabled { get; set; } = false;

    #endregion

    #region [Methods :: EventHandlers :: Class]

    public MVPLoginContentPage()
    {
        InitializeComponent();
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        InitializeControls();
        //InitializeConnectivityManagerBottomSheet();

        //if (string.IsNullOrWhiteSpace(this.UsernameEntry.Text) || string.IsNullOrWhiteSpace(this.PasswordEntry.Text))
        //{
        //    this.LoginButton.BackgroundColor = Color.FromRgba(206, 211, 215, 255);
        //    this.LoginButton.TextColor = Colors.White;
        //    this.LoginButton.IsEnabled = false;
        //}
        //else
        //{
        //    this.LoginButton.BackgroundColor = Colors.White;
        //    this.LoginButton.TextColor = Color.FromRgba(0, 98, 114, 255);
        //    this.LoginButton.IsEnabled = true;
        //}
        InitializeDeviceInformation();
        InitializeStorage();       
    }

    private void ContentPage_Unloaded(object sender, EventArgs e)
    {
    }

    private void InitializeControls()
    {
        this.LostInternetBottomSheet.HalfExpandedRatio = 0.9;

        if (string.IsNullOrWhiteSpace(this.UsernameEntry.Text) || string.IsNullOrWhiteSpace(this.PasswordEntry.Text))
        {
            this.LoginButton.BackgroundColor = Color.FromRgba(206, 211, 215, 255);
            this.LoginButton.TextColor = Colors.White;
            this.LoginButton.IsEnabled = false;
        }
        else
        {
            this.LoginButton.BackgroundColor = Colors.White;
            this.LoginButton.TextColor = Color.FromRgba(0, 98, 114, 255);
            this.LoginButton.IsEnabled = true;
        }
    }

    private void InitializeConnectivityManagerBottomSheet()
    {

        if (App.connectivityManager != null)
        {
            App.connectivityManager.ClearOpenNoInternetConnectionBottomSheetSubscription();

            if (App.connectivityManager.OpenNoInternetConnectionBottomSheetHasSubscription() == false)
            {
                App.connectivityManager.OpenNoInternetConnectionBottomSheet -= ConnectivityManager_ShowNoInternetErrorBottomSheet;
                App.connectivityManager.OpenNoInternetConnectionBottomSheet += ConnectivityManager_ShowNoInternetErrorBottomSheet;
            }
        }
    }

    #endregion

    #region [Methods :: EventHandlers :: Controls]

    private async void LogInButton_Clicked(object sender, EventArgs e)
    {
        await LogInUser();
    }

    private async void OnUsernameKeyboardFocus(object sender, FocusEventArgs e)
    {
#if IOS || ANDROID
        if (KeyboardExtensions.IsSoftKeyboardShowing(this.UsernameEntry) == false)
        {
            await KeyboardExtensions.ShowKeyboardAsync(this.UsernameEntry, new CancellationToken());

        }
        //this.RootScrollView.IsEnabled = true;
        if (this.VariableRow != null)
        {
            this.VariableRow.Height = new GridLength(1, GridUnitType.Star);
        }

#endif
    }

    private async void OnUsernameKeyboardUnfocus(object sender, FocusEventArgs e)
    {
#if IOS || ANDROID
        if (KeyboardExtensions.IsSoftKeyboardShowing(this.UsernameEntry) == true)
        {
            await KeyboardExtensions.HideKeyboardAsync(this.UsernameEntry, new CancellationToken());
        }
        if (this.VariableRow != null)
        {
            this.VariableRow.Height = new GridLength(0, GridUnitType.Star);
        }
#endif
    }

    private async void OnPasswordKeyboardFocus(object sender, FocusEventArgs e)
    {
#if IOS || ANDROID
        if (KeyboardExtensions.IsSoftKeyboardShowing(this.PasswordEntry) == false)
        {
            await KeyboardExtensions.ShowKeyboardAsync(this.PasswordEntry, new CancellationToken());

        }
        //this.RootScrollView.IsEnabled = true;
        if (this.VariableRow != null)
        {
            this.VariableRow.Height = new GridLength(1, GridUnitType.Star);
        }
#endif
    }

    private async void OnPasswordKeyboardUnfocus(object sender, FocusEventArgs e)
    {
#if IOS || ANDROID
        if (KeyboardExtensions.IsSoftKeyboardShowing(this.PasswordEntry) == true)
        {
            await KeyboardExtensions.HideKeyboardAsync(this.PasswordEntry, new CancellationToken());
        }
        if (this.VariableRow != null)
        {
            this.VariableRow.Height = new GridLength(0, GridUnitType.Star);
        }
#endif
    }

    public void CheckEnableButton(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(this.UsernameEntry.Text) || string.IsNullOrWhiteSpace(this.PasswordEntry.Text))
        {
            this.LoginButton.BackgroundColor = Color.FromRgba(206, 211, 215, 255);
            this.LoginButton.TextColor = Colors.White;
            this.LoginButton.IsEnabled = false;
        }
        else
        {
            this.LoginButton.BackgroundColor = Colors.White;
            this.LoginButton.TextColor = Color.FromRgba(0, 98, 114, 255);
            this.LoginButton.IsEnabled = true;
        }
    }

    private void ConnectivityManager_ShowNoInternetErrorBottomSheet(object sender, EventArgs eventArgs)
    {
        ShowNoInternetErrorBottomSheet();
    }

    private void CloseLostInternetErrorBottomSheetButton_Clicked(object sender, EventArgs e)
    {
        this.LostInternetBottomSheet.State = BottomSheetState.Hidden;
    }

    private void GoToSettingBottomSheetButton_Clicked(object sender, EventArgs e)
    {
        this.LostInternetBottomSheet.State = BottomSheetState.Hidden;
        GoToSettingApplication();
    }

    #endregion

    #region [Methods :: Tasks]   

    private async Task LogInUser()
    {
        try
        {

            if (string.IsNullOrWhiteSpace(this.UsernameEntry.Text) == true && string.IsNullOrWhiteSpace(this.PasswordEntry.Text) == true)
            {
                this.FailedAttempt.IsVisible = true;
                return;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(this.UsernameEntry.Text) == true)
                {
                    this.FailedAttempt.IsVisible = true;
                    return;
                }

                if (string.IsNullOrWhiteSpace(this.PasswordEntry.Text) == true)
                {
                    this.FailedAttempt.IsVisible = true;
                    return;
                }
            }

            this.ActivityIndicatorFrame.IsVisible = true;

            string username = this.UsernameEntry.Text;
            string password = this.PasswordEntry.Text;

            // string username = "testthomas_shelby@yopmail.com";
            //   string password = "Aa12345!";

            var login = await UserHandler.LoginUserManually(username, password);
            if (login.isSuccess)
            {
                await UserHandler.SaveUser(new MinUserModel() { FkFederatedUser = login.FkFederatedUser,ID = login.UserId, NM = login.UserName });
                await UserHandler.LoginUser(new MinUserModel() { FkFederatedUser = login.FkFederatedUser, ID = login.UserId, NM = login.UserName });

                try
                {
                    AirMemoryCache.GetUserHistory();
                }
                catch { }

                if (App.Current.MainPage.Navigation.NavigationStack.First().GetType().Name != "MainPage")
                {
                    App.Current.MainPage = new NavigationPage(new MainPage());
                }
                await Navigation.PopToRootAsync();
                try
                {
#if !WINDOWS
                    //  await UserMiddleware.RegisterDevice(await PushRegistration.CheckPermission(), PushRegistration.GetPlatform());
                    if (PushNavigationHelper.RootPage != null)
                    {
                        if (!await MauiApp1.Pages.Index.CheckNotificationPermissions())
                        {
                            PushNavigationHelper.RootPage.OpenRequestNotificationPermissionsPopup();
                        }
                    }
#endif

                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                //await DisplayAlert("Login Error", "Login Error", "OK");
                this.UsernameEntry.Text = "";
                this.PasswordEntry.Text = "";
                this.FailedAttempt.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            //await DisplayAlert("Login Error", ex.Message + " " + ex.StackTrace, "OK");
            this.UsernameEntry.Text = "";
            this.PasswordEntry.Text = "";
            this.FailedAttempt.IsVisible = true;
        }
        finally
        {
            this.ActivityIndicatorFrame.IsVisible = false;
        }
    }

    private void InitializeStorage()
    {
        ParentMiddleWare.ConnectedDeviceDataStorageManager.ClearProperties();
        Preferences.Default.Clear();
        try
        {
            SecureStorage.Default.RemoveAll();


        }
        catch (Exception ex)
        {

        }
    }

    private async void InitializeDeviceInformation()
    {
        Guid deviceId = Guid.Empty;
        try
        {
            deviceId = await DeviceStorageManager.GetDeviceIdStorageData();

            if(deviceId == Guid.Empty)
            {
                if (App.deviceInformationManager.DeviceId != Guid.Empty)
                {
                    DeviceStorageManager.SaveDeviceIdData(App.deviceInformationManager.DeviceId);
                }
                else
                {
                    App.deviceInformationManager.CreateDeviceId();
                    DeviceStorageManager.SaveDeviceIdData(App.deviceInformationManager.DeviceId);
                }
            }
            else
            {
                App.deviceInformationManager.DeviceId = deviceId;
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

    private async void ClosePage()
    {
        await Navigation.PopToRootAsync();
    }

    private void ShowNoInternetErrorBottomSheet()
    {
        this.LostInternetBottomSheet.State = BottomSheetState.HalfExpanded;
    }

    private void GoToSettingApplication()
    {
        AppInfo.Current.ShowSettingsUI();
    }

    #endregion

    
}