using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserApi.Net7;
using CommunityToolkit.Maui.Core.Platform;
using MauiApp1.Pages;
using MauiApp1._Push;

namespace MauiApp1.Areas.Security.Views;

public partial class MVPLoginContentPage : ContentPage
{
    public bool IsLoginButtonEnabled { get; set; } = false;

    public MVPLoginContentPage()
    {
        InitializeComponent();
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
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

    private void ContentPage_Unloaded(object sender, EventArgs e)
    {
    }

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
        if(this.VariableRow != null)
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
            this.LoginButton.BackgroundColor = Color.FromRgba(206, 211, 215,255);
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

            var login = await MauiApp1.Pages.Index.LoginUserManually(username, password);
            if (login.isSuccess)
            {
                await MauiApp1.Pages.Index.SaveUser(login);
                await MauiApp1.Pages.Index.LoginUser(login);
                //await Navigation.PushModalAsync(new MainPage());
                //System.Diagnostics.Debug.WriteLine(App.Current.MainPage.Navigation.NavigationStack.First().GetType().Name);
                if(App.Current.MainPage.Navigation.NavigationStack.First().GetType().Name != "MainPage")
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

    private async void ClosePage()
    {
        await Navigation.PopToRootAsync();
    }
}