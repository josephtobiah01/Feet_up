using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace MauiApp1.Business
{
    public class ConnectivityManager
    {
        #region [Fields]

        private NetworkAccess _pastAccessType;

        public event EventHandler OpenNoInternetConnectionBottomSheet;

        #endregion

        #region [Methods :: EventHandlers :: Class]
        public ConnectivityManager() =>
        Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;

        ~ConnectivityManager() =>
            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;

        #endregion

        #region [Methods :: Tasks]

        void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            try
            {
                CheckInternetConnection();

                foreach (var item in e.ConnectionProfiles)
                {
                    switch (item)
                    {
                        case ConnectionProfile.Bluetooth:
                            // Console.Write("Bluetooth");
                            break;
                        case ConnectionProfile.Cellular:
                            //  Console.Write("Cell");
                            break;
                        case ConnectionProfile.Ethernet:
                            //    Console.Write("Ethernet");
                            break;
                        case ConnectionProfile.WiFi:
                            //  Console.Write("WiFi");
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region [Public Methods :: Tasks]
        public void CheckInternetConnection()
        {
            NetworkAccess accessType = Connectivity.Current.NetworkAccess;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            ToastDuration duration = ToastDuration.Short;
            double fontSize = 14;
            string toastText = string.Empty;

            if (accessType != NetworkAccess.Internet)
            {
                // Connection to internet is not available
                if (OpenNoInternetConnectionBottomSheet != null)
                {
                    OpenNoInternetConnectionBottomSheet.Invoke(this, new EventArgs());
                }
                else if (App.Current.MainPage != null)
                {

                    App.Current.MainPage.DisplayAlert("Offline",
                        "Please check the internet connection and try again.", "OK");
                }
                else
                {

                }

            }
            else if (accessType == NetworkAccess.Internet)
            {
                if (_pastAccessType != NetworkAccess.Unknown && _pastAccessType != NetworkAccess.Internet)
                {
                    if (App.Current.MainPage != null)
                    {
                        toastText = "Online, Internet connection was restored";

                        IToast toast = Toast.Make(toastText, duration, fontSize);

                        toast.Show(cancellationTokenSource.Token);

                        //App.Current.MainPage.DisplayAlert("Online",
                        //    "Internet connection was restored", "OK");
                    }
                }


            }
            _pastAccessType = accessType;
        }

        public bool OpenNoInternetConnectionBottomSheetHasSubscription()
        {
            bool HasSubscription = false;

            if (OpenNoInternetConnectionBottomSheet != null)
            {
                if (OpenNoInternetConnectionBottomSheet.GetInvocationList().Count() > 0)
                {
                    HasSubscription = true;
                }
            }

            return HasSubscription;
        }

        public void ClearOpenNoInternetConnectionBottomSheetSubscription()
        {
            OpenNoInternetConnectionBottomSheet = null;
        }

        #endregion

    }
}
