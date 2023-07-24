using FeedApi.Net7.Models;
using MauiApp1.Areas.BarcodeScanning.Views;
using MauiApp1.Areas.Security.Views;
using MauiApp1.Areas.Chat.Views;
using MauiApp1.Areas.Profile.Views;
using MauiApp1.Business.DeviceServices;
using MauiApp1._Push;
using ParentMiddleWare;
using UserApi.Net7;
using MauiApp1.Areas.Test.Views;
using MauiApp1.Areas.Overview.Views;
using FitnessData.Client.Business;
using FitnessData.Common;
using Newtonsoft.Json;
using System.Text;
using MauiApp1.Business;
using MauiApp1.Helpers;
using Microsoft.AspNetCore.Components;

namespace MauiApp1.Shared
{
    public partial class NavMenu
    {

        #region [Fields]
        public string DisplayUserPopup = "none";
        private bool collapseNavMenu = true;
        public string LoginLogout = "Login";
        private string _connectedAppsText = "Connect Application";
        public static bool isLoggedIn = false;

        private bool _hideMessageButton = true;
        private bool _hideConnectedAppButton = true;
        private string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        #endregion

        #region[Initialization]
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            
            if (firstRender)
            {
#if IOS || ANDROID
                //NotificationCounter.Default.SetNotificationCount(0);
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
        protected virtual async void RefreshMenu_OnRefresh(object sender, EventArgs e)
        {
            RefreshMenu();
        }
        private void ConnectedApps_Clicked()
        {
            //ConnectApplicationToGoogleFit();
        }

        #endregion

        #region [Methods :: Tasks]

        

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
        public async void OpenAddNewPopup()
        {
            if (PushNavigationHelper.RootPage != null)
            {
               await PushNavigationHelper.RootPage.OpenAddNewPopup();
            }
        }
        public async Task GoToProfilePage()
        {
            await App.Current.MainPage.Navigation.PushAsync(new ViewProfileContentPage());
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
                _hideConnectedAppButton = false;
                LoginLogout = "Logout";
                isLoggedIn = true;
            }
            else
            {
                _hideMessageButton = true;
                _hideConnectedAppButton = true;
                LoginLogout = "Login";
                isLoggedIn = false;
            }

            StateHasChanged();
        }

        private void ClearFitnessServiceStorage()
        {
            Preferences.Default.Set("fitness_service", string.Empty);
        }


        #endregion

        #region [Fields :: Public]

        [Parameter]
        public bool AddButtonHidden { get; set; }


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
            await App.Current.MainPage.Navigation.PushAsync(new TestContentPage());
        }
        #endregion

    }
}
