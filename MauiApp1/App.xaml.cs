using MauiApp1._Push;
using Shiny.Hosting;
using Shiny.Notifications;
using MauiApp1.Areas.Security.Views;
using ParentMiddleWare;
using System.Security.AccessControl;
#if ANDROID
using Plugin.FirebasePushNotification;
#endif

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MauiApp1
{
    public partial class App : Application
    {

        // private readonly BackgroundWorker _worker;
        public App()
        {
            try
            {
#if IOS
                AppDelegate.blockNotifications = true;
                AppDelegate.BloclTime = DateTime.Now;
#endif
                InitializeComponent();

                MiddleWare.Setup();

                #region Notifications
                var notifications = Host.Current.Services.GetService<INotificationManager>();

                notifications.AddChannel(new Channel
                {
                    Identifier = "General",
                    Importance = ChannelImportance.High,
                    Actions =
                {
                    new ChannelAction
                    {
                        Identifier = "General",
                        Title = "General",
                        ActionType = ChannelActionType.OpenApp
                    }
                }
                });

                notifications.AddChannel(new Channel
                {
                    Identifier = "Notifications",
                    Importance = ChannelImportance.High,
                    Actions =
                {
                    new ChannelAction
                    {
                        Identifier = "Notifications",
                        Title = "GeneNotificationsral",
                        ActionType = ChannelActionType.OpenApp
                    }
                }
                });

                notifications.AddChannel(new Channel
                {
                    Identifier = "Miscellaneous",
                    Importance = ChannelImportance.High,
                    Actions =
                {
                    new ChannelAction
                    {
                        Identifier = "Miscellaneous",
                        Title = "GeneMiscellaneousral",
                        ActionType = ChannelActionType.OpenApp
                    }
                }
                });
                #endregion


                Task<bool> task = Task.Run<bool>(async () => await MauiApp1.Pages.Index.SetupUser());
                var serviceResult = task.Result;
                if (serviceResult == true) { }


                // sync by design
                AirMemoryCache.GetUserHistory();
                //ViewProfile.IsAppConnected();
#if IOS
                //  var _notificationManager = Host.Current.Services.GetService<INotificationManager>();

                ////  _notificationManager.ClearChannels();
                //  _notificationManager.AddChannel(
                //new Shiny.Notifications.Channel()
                //{
                //    Importance = Shiny.Notifications.ChannelImportance.Critical,
                //    Description = "General",
                //    Identifier = "General",

                //    Sound = ChannelSound.None,
                //});

#endif

#if ANDROID

            ////Change for your default notification channel id here
            //FirebasePushNotificationManager.DefaultNotificationChannelId = "Notifications";

            ////Change for your default notification channel name here
            //FirebasePushNotificationManager.DefaultNotificationChannelName = "Notifications";


            CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine($"TOKEN : {p.Token}");
            };
            // Push message received event  
         
            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {

            };

            CrossFirebasePushNotification.Current.OnNotificationAction += (s, p) =>
            {

            };

            CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
            {
                try
                {
                    var action = p.Data.Where(t => t.Key == "aaction").FirstOrDefault();
                    var param = p.Data.Where(t => t.Key == "param1").FirstOrDefault();
                    string command1 = "";
                    if (param.IsNullOrDefault() || param.Value == null) command1 = "";
                    else command1 = param.Value.ToString();

                    if (!action.IsNullOrDefault())
                    {
                        //  await PushNavigationHelper.HandleNotificationTab(action.Value.ToString(), command1);
                        Task task = Task.Run(async () => await PushNavigationHelper.HandleNotificationTab(action.Value.ToString(), command1));
                    }
                }
                catch { }
            };

            CrossFirebasePushNotification.Current.OnNotificationError += (s, p) =>
            {

            };
#endif
                //MainPage = new NavigationPage(new MainPage());
                //if (ParentMiddleWare.MiddleWare.UserID <= 0)
                //{
                //    App.Current.MainPage.Navigation.PushAsync(new MVPLoginContentPage());
                //}


                if (ParentMiddleWare.MiddleWare.UserID <= 0)
                {
                    MainPage = new NavigationPage(new MVPLoginContentPage());
                }
                else
                {
                    MainPage = new NavigationPage(new MainPage());
                }




                NetworkAccess accessType = Connectivity.Current.NetworkAccess;

                if (accessType != NetworkAccess.Internet)
                {
                    App.Current.MainPage.DisplayAlert("Offline", "Please check the internet connection and try again.",
                              "OK");
                }
                ConnectivityTest ctest = new ConnectivityTest();


            }
            catch (Exception ex)
            {
                try
                {
                    // shell.current.displayalert
                }
                catch (Exception ex2)
                {
                    //Log error
               //     window.Page.DisplayAlert("Application StartUp Error", "An error occured while trying to login on start." + ex.Message + ex.StackTrace, "OK");

                }
                finally
                {

                }
            }
        }


        protected override Window CreateWindow(IActivationState activationState)
        {
            Window window = base.CreateWindow(activationState);

            try
            {
            }
            catch (Exception ex)
            {
                //Log error
                window.Page.DisplayAlert("Application StartUp Error", "An error occured while trying to login on start." + ex.Message + ex.StackTrace, "OK");

            }
            finally
            {

            }

            return window;
        }
    }
    public class ConnectivityTest
    {
        public ConnectivityTest() =>
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;

        ~ConnectivityTest() =>
            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;

        void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            //if (e.NetworkAccess == NetworkAccess.ConstrainedInternet)
            //{
            //    //App.Current.MainPage.DisplayAlert("Offline", "Please check the internet connection and try again.",
            //    //"OK");
            //    //Console.WriteLine("Internet access is available but is limited.");
            //}

            if (e.NetworkAccess != NetworkAccess.Internet)
            {
                App.Current.MainPage.DisplayAlert("Offline", "Please check the internet connection and try again.",
                "OK");
            }
                //Console.WriteLine("Internet access has been lost.");

            // Log each active connection
          //  Console.Write("Connections active: ");

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
    }


}
