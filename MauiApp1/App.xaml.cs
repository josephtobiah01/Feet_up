using MauiApp1._Push;
using MauiApp1.Areas.Security.Views;
using MauiApp1.Business;
using MauiApp1.Business.DeviceServices;
using ParentMiddleWare;
using MauiApp1.Business.UserServices;
using UserApi.Net7;
#if ANDROID
using Plugin.FirebasePushNotification;
using Firebase;
#endif
#if IOS
using Shiny.Hosting;
using Shiny.Notifications;
#endif

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MauiApp1
{
    public partial class App : Application
    {
        //private static ConnectivityTest ctest;
        public static ConnectivityManager connectivityManager;
        public static AlertBottomSheetManager alertBottomSheetManager;

        public static DeviceInformationManager deviceInformationManager;

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

#if IOS
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
                        ActionType = ChannelActionType.None
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
                        ActionType = ChannelActionType.None
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
                        ActionType = ChannelActionType.None
                    }
                }
                });
                #endregion
#endif

                Task<bool> task = Task.Run<bool>(async () => await UserHandler.SetupUser());
                var serviceResult = task.Result;

                // sync by design
                //  try
                // {
                //   Task<bool> task2 = Task.Run<bool>(async () => await SetupFireBase());
                // }
                // catch { }

                try
                {
                    AirMemoryCache.GetUserHistory();
                }
                catch { }

                try
                {
                    alertBottomSheetManager = new AlertBottomSheetManager();

                    connectivityManager = new ConnectivityManager();

                    deviceInformationManager = new DeviceInformationManager();                    
                }
                catch (Exception e)
                {

                }

                if (ParentMiddleWare.MiddleWare.FkFederatedUser == string.Empty)
                {
                    MainPage = new NavigationPage(new MVPLoginContentPage());
                }
                else
                {
                    MainPage = new NavigationPage(new MainPage());
                }

            }
            catch (Exception ex)
            {

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

#if ANDROID
        public static async Task<bool> SetupFireBase()
        {
            try
            {
                //   Task<bool> task = Task.Run<bool>(async () => await MauiApp1.Pages.Index.SetupUser());
                // var serviceResult = task.Result;

                // await MauiApp1.Pages.Index.SetupUser();

                //Set the default notification channel for your app when running Android Oreo  

                //Change for your default notification channel id here  
                FirebasePushNotificationManager.DefaultNotificationChannelId = $"General";

                ////Change for your default notification channel name here  
                FirebasePushNotificationManager.DefaultNotificationChannelName = "General";
                FirebasePushNotificationManager.DefaultNotificationChannelImportance = Android.App.NotificationImportance.High;

                //FirebasePushNotificationManager.IconResource = Resource.Drawable.pushIcon;

                //Sets the sound  uri will be used for the notification
                //FirebasePushNotificationManager.Android.Net.Uri SoundUri { get; set; }

                //Sets the color will be used for the notification
                //  FirebasePushNotificationManager.Color = new Android.Graphics.Color(240, 8, 7);

                //Sets the default notification channel importance for Android O
                // FirebasePushNotificationManager.DefaultNotificationChannelImportance = NotificationImportance.Max;

                // FirebasePushNotificationManager.ShouldShowWhen = true;
                // FirebasePushNotificationManager.UseBigTextStyle = true;
                //   FirebasePushNotificationManager.LargeIconResource = Resource.Drawable.air_icon_90x32;
                //  FirebasePushNotificationManager.NotificationActivityFlags = ActivityFlags.

                FirebasePushNotificationManager.NotificationActivityFlags = Android.Content.ActivityFlags.SingleTop;



                //If debug you should reset the token each time.  

                string AppId = "1:368008538066:android:b25aacfaa968fe44130a0b";
                string SenderId = "368008538066";
                string ProjectId = "age-in-reverse-longevity";
                string ApiKey = "AIzaSyAkP7jzgygDHerCG7PVU5Lo3l-nhbLYrLk";


                var options = new FirebaseOptions.Builder()
                     .SetApplicationId(AppId)
                     .SetProjectId(ProjectId)
                     .SetApiKey(ApiKey)
                     .SetGcmSenderId(SenderId)
                .Build();


                MyPushNotificationHandler myHandler = new MyPushNotificationHandler();

                CrossFirebasePushNotification.Android.NotificationHandler = myHandler;
                //var channelId = $"General";
                //var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                //var channel = new NotificationChannel(channelId, "General", NotificationImportance.High);
                //channel.EnableVibration(true);
                //notificationManager.CreateNotificationChannel(channel);


                FirebaseApp.InitializeApp(Android.App.Application.Context, options);
                //FirebaseMessaging.Instance.AutoInitEnabled = true; //.SetAutoInitEnabled(true);
                //  FirebaseMessaging.Instance.SubscribeToTopic("subTopic");


                FirebasePushNotificationManager.Initialize(Android.App.Application.Context, false);
                // FirebasePushNotificationManager.Initialize(this, false, true, false);

                ////Change for your default notification channel id here
                //FirebasePushNotificationManager.DefaultNotificationChannelId = "Notifications";

                ////Change for your default notification channel name here
                //FirebasePushNotificationManager.DefaultNotificationChannelName = "Notifications";


                CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
                {
                    //     System.Diagnostics.Debug.WriteLine($"TOKEN : {p.Token}");
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
                        if (param.Value == null) command1 = "";
                        else command1 = param.Value.ToString();

                        if (action.Value != null)
                        {
                            var task = PushNavigationHelper.HandleNotificationTab(action.Value.ToString(), command1);
                            //  var k = task.Result;
                            //  task.Result;
                            //Task<bool> task = Task<bool>.Run(async () => await PushNavigationHelper.HandleNotificationTab(action.Value.ToString(), command1));
                            //var m = task.Result;


                            //  k..Wait();
                            //    Task task = Task.Run(async () => await PushNavigationHelper.HandleNotificationTab(action.Value.ToString(), command1));
                        }
                    }
                    catch (Exception e)
                    {

                    }
                };

                CrossFirebasePushNotification.Current.OnNotificationError += (s, p) =>
                {

                };

                UserMiddleware.RegisterDevice(await PushRegistration.CheckPermission(), PushRegistration.GetPlatform());
            }
            catch (Exception e)
            {

            }
            finally
            {
            }
            return true;
       
    }
#endif

    }
}
        

