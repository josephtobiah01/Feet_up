using FeedApi.Net7.Models;
using MauiApp1.Areas.BarcodeScanning.Views;
using MauiApp1.Areas.Security.Views;
using MauiApp1.Areas.Mindfulness.Views;
using MauiApp1.Areas.Chat.Views;
using MauiApp1.Business.DeviceServices;
using Microsoft.Maui.Controls;
using ParentMiddleWare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserApi.Net7.Models;
using Microsoft.JSInterop;
using UserApi.Net7;
#if ANDROID || IOS
using MauiApp1.Areas.Badges.Models;
#endif

namespace MauiApp1.Shared
{
    public partial class NavMenu
    {

        #region [Fields]
        public string DisplayUserPopup = "none";
        private bool collapseNavMenu = true;
        public string LoginLogout = "Login";
        public static bool isLoggedIn = false;

        private bool _hideMessageButton = true;
        private string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        #endregion
        #region[Initialization]
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            
            if (firstRender)
            {
#if IOS || ANDROID
                NotificationCounter.Default.SetNotificationCount(0);
#endif
                bool IssignedInUser = MiddleWare.UserID > 0;
              if (IssignedInUser)
                {
                    LoginLogout = "Logout";
                    isLoggedIn = true;
                }
                else
                {
                    LoginLogout = "Login"; 
                    isLoggedIn = false;
                }

                if (HTMLBridge.RefreshMenu != null)
                {
                    HTMLBridge.RefreshMenu -= RefreshMenu_OnRefresh;
                }

                HTMLBridge.RefreshMenu += RefreshMenu_OnRefresh;
                RefreshMenu();
            }
        }
        #endregion
        #region [Methods :: EventHandlers :: Class]

        public NavMenu() 
        { 
        }

        #endregion

        #region [Methods :: EventHandlers :: Controls]
        private void ProfilePictureButton_Click()
        {
            ShowUserMenu();
        }

        protected virtual async void RefreshMenu_OnRefresh(object sender, EventArgs e)
        {
            RefreshMenu();
        }

        #endregion

        #region [Methods :: Tasks]

        public async void Login()
        {
            bool IssignedInUser = MiddleWare.UserID > 0;
            if (IssignedInUser)
            {
                SignOutUser();
                CloseUserPopup();
            }
            else
            {
                await App.Current.MainPage.Navigation.PushAsync(new ViewLoginContentPage());
                ////temporary
                //LoginLogout = "Logout";
                CloseUserPopup();
                StateHasChanged();
            }            
        }
        private async void ShowUserMenu()
        {

            string[] menu = new string[1];

            bool  IssignedInUser = MiddleWare.UserID > 0;

            if (IssignedInUser)
            {
                menu[0] = "Sign out";
            }
            else
            {
                menu[0] = "Sign in";

            }          

            string action = await App.Current.MainPage.DisplayActionSheet("Menu", "Cancel", null,
                 menu);

            switch (action)
            {
                case "Sign out":

                    SignOutUser();
                    //await App.Current.MainPage.Navigation.PushModalAsync(new MainPage());
                    break;

                case "Sign in":

                    await App.Current.MainPage.Navigation.PushAsync(new ViewLoginContentPage());
                    break;               

                default:                   
                    break;
            }


        }

        private async void SignOutUser()
        {

            ShowLoadingActivityIndicator();

            bool userSignedOut = await App.Current.MainPage.DisplayAlert("Sign Out", "Are you sure you want to sign out","Sign out", "Cancel");
       
            if (userSignedOut == true)
            {
                long userId = MiddleWare.UserID;
                bool userSignedOutSuccessful = MauiApp1.Pages.Index.LogOffuser();

                if(userSignedOutSuccessful == true)
                {
                    try
                    {
                        await UserMiddleware.UnRegisterDevice(await PushRegistration.CheckPermission(), userId, PushRegistration.GetPlatform());
                    }
                    catch (Exception ex)
                    {

                    }
                    if (HTMLBridge.RefreshData != null)
                    {
                        HTMLBridge.RefreshData.Invoke(this, null);
                    }
                }
                //await App.Current.MainPage.Navigation.PushModalAsync(new MainPage());
                LoginLogout = "Login";
                isLoggedIn = false;
                CloseUserPopup();
                StateHasChanged();
            }
            else
            {
                HideLoadingActivityIndicator();
                StateHasChanged();
            }
        }

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }
        public async void GoToChatPage()
        {
            bool userSignedIn = MiddleWare.UserID > 0;

            switch (userSignedIn)
            {
                case true:

                    await Application.Current.MainPage.Navigation.PushAsync(new ViewHybridChatContentPage());
                    break;

                case false:

                    await App.Current.MainPage.DisplayAlert("Access Denied", "You do not have access to this page. Please log in to access this page.", "OK");
                    break;
            }

            
        }

        public void OpenUserPopup()
        {
            RefreshMenu();
            DisplayUserPopup = "inline";
        }
        public void CloseUserPopup()
        {
            DisplayUserPopup = "none";
        }
        private async void GotoScanPage()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new BarcodeScannerContentPage());
        }

        private void ShowLoadingActivityIndicator()
        {
            if (HTMLBridge.MainPageBlackStackLayout != null)
            {
                HTMLBridge.MainPageBlackStackLayout.IsVisible = true;
            }
            if (HTMLBridge.MainPageLoadingActivityIndicator != null)
            {
                HTMLBridge.MainPageLoadingActivityIndicator.IsVisible = true;
            }
        }

        private void HideLoadingActivityIndicator()
        {
            if (HTMLBridge.MainPageBlackStackLayout != null)
            {
                HTMLBridge.MainPageBlackStackLayout.IsVisible = false;
            }
            if (HTMLBridge.MainPageLoadingActivityIndicator != null)
            {
                HTMLBridge.MainPageLoadingActivityIndicator.IsVisible = false;
            }
        }


        private void RefreshMenu()
        {
            bool userSignedIn = MiddleWare.UserID > 0;
            if (userSignedIn == true)
            {
                _hideMessageButton = false;
                LoginLogout = "Logout";
                isLoggedIn = true;
            }
            else
            {
                _hideMessageButton = true;
                LoginLogout = "Login";
                isLoggedIn = false;
            }
            StateHasChanged();
        }

        #endregion
        #region[DEBUG]
        public bool isDebug()
        {
            #if DEBUG
                return true;
            #else
                return false;
            #endif
        }

        public async void GoToTestPage()
        {
            await App.Current.MainPage.Navigation.PushAsync(new ViewChatDetailPageVer2());
        }
        #endregion
    }
}
