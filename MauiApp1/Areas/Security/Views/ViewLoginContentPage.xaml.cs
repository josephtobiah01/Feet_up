using UserApi.Net7;
using UserApi.Net7.Models;

namespace MauiApp1.Areas.Security.Views;

public partial class ViewLoginContentPage : ContentPage
{

    #region [Fields]
    #endregion

    #region [Methods :: EventHandlers :: Class]

    public ViewLoginContentPage()
    {
        InitializeComponent();
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        //Initialize Data Here
        //
        //this.UsernameEntry.Text = "DominikTest";
        //this.PasswordEntry.Text = "Aa12345!";
    }

    private void ContentPage_Unloaded(object sender, EventArgs e)
    {

    }


    #endregion

    #region [Methods :: EventHandlers :: Controls]

    private async void LogInButton_Clicked(object sender, EventArgs e)
    {
        await LogInUser();
    }

    private void CancelButton_Clicked(object sender, EventArgs e)
    {
        ClosePage();
    }

    #endregion

    #region [Methods :: Tasks]

    private async Task LogInUser()
    {
        try
        {

            if (string.IsNullOrWhiteSpace(this.UsernameEntry.Text) == true && string.IsNullOrWhiteSpace(this.PasswordEntry.Text) == true)
            {
                await DisplayAlert("Login Error", "Username and password cannot be empty.", "OK");
                return;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(this.UsernameEntry.Text) == true)
                {
                    await DisplayAlert("Login Error", "Username cannot be empty.", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(this.PasswordEntry.Text) == true)
                {
                    await DisplayAlert("Login Error", "Password cannot be empty.", "OK");
                    return;
                }
            }            

            this.ActivitiyIndicatorFrame.IsVisible = true;

            string username = this.UsernameEntry.Text;
            string password = this.PasswordEntry.Text;

            var login = await MauiApp1.Pages.Index.LoginUserManually(username, password);
            if (login.isSuccess)
            {
                await MauiApp1.Pages.Index.SaveUser(login);
                await MauiApp1.Pages.Index.LoginUser(login);
                //await Navigation.PushModalAsync(new MainPage());
                await Navigation.PopToRootAsync();
                try
                {
                    await UserMiddleware.RegisterDevice(await PushRegistration.CheckPermission());
                }
                catch(Exception ex)
                {

                }
            }
            else
            {
                await DisplayAlert("Login Error", "Login Error", "OK");
            }
        }
        catch(Exception ex)
        {
            await DisplayAlert("Login Error", ex.Message + " " + ex.StackTrace, "OK");
        }
        finally 
        {
            this.ActivitiyIndicatorFrame.IsVisible = false;
        }

        
        
    }


    private async void ClosePage()
    {
        await Navigation.PopToRootAsync();
    }

    #endregion

    
}