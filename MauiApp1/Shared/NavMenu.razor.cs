using MauiApp1._Push;
using MauiApp1.Areas.BarcodeScanning.Views;
using MauiApp1.Areas.Chat.Views;
using MauiApp1.Areas.Profile.Views;
using MauiApp1.Areas.Test.Views;
using MauiApp1.Business.DeviceServices;
using MauiApp1.Interfaces;
using MauiApp1.Pages.Chat;
using MauiApp1.Services;
using Microsoft.AspNetCore.Components;
using ParentMiddleWare;

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
                bool IssignedInUser = MiddleWare.FkFederatedUser != string.Empty;
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
            bool userSignedIn = MiddleWare.FkFederatedUser != string.Empty;

            switch (userSignedIn)
            {
                case true:
                    ISelectedImageService selectedImageService = new SelectedImageService();
                    ViewHybridChatContentPage viewHybridChatContentPage = new ViewHybridChatContentPage(selectedImageService);
                    await Application.Current.MainPage.Navigation.PushAsync(viewHybridChatContentPage);
                    break;

                case false:

                    ShowAlertBottomSheet("Access Denied", "You do not have access to this page. Please log in to access this page.", "OK");
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
            bool userSignedIn = MiddleWare.FkFederatedUser != string.Empty;
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

        private void ShowAlertBottomSheet(string title, string message, string cancelMessage)
        {
            if (App.alertBottomSheetManager != null)
            {
                App.alertBottomSheetManager.ShowAlertMessage(title, message, cancelMessage);
            }
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
