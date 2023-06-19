using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using Firebase.Messaging;
using MauiApp1.Areas.Chat.Views;
using Plugin.Firebase.CloudMessaging;
using Plugin.FirebasePushNotification;
using Shiny.Notifications;
using static Microsoft.Maui.ApplicationModel.Platform;
using Intent = Android.Content.Intent;
using NotificationManager = Android.App.NotificationManager;

namespace MauiApp1
{
    //[Activity(
    //    Theme = "@style/Maui.SplashTheme",
    //    MainLauncher = true,
    //    ConfigurationChanges =
    //        ConfigChanges.ScreenSize |
    //        ConfigChanges.Orientation |
    //        ConfigChanges.UiMode |
    //        ConfigChanges.ScreenLayout |
    //        ConfigChanges.SmallestScreenSize |
    //        ConfigChanges.Density
    //)]
    //[IntentFilter(
    //    new[] {
    //    ShinyPushIntents.NotificationClickAction,
    //    ShinyNotificationIntents.NotificationClickAction
    //    }
    //)]
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : MauiAppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            FirebasePushNotificationManager.ProcessIntent(this, Intent);
            //HandleIntent(Intent);
            // CreateNotificationChannelIfNeeded();
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            HandleIntent(intent);
        }

        private static void HandleIntent(Intent intent)
        {
            FirebaseCloudMessagingImplementation.OnNewIntent(intent);
        }

        private void CreateNotificationChannelIfNeeded()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                CreateNotificationChannel();
            }
        }

        private void CreateNotificationChannel()
        {
            var channelId = $"{PackageName}.General";
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            var channel = new NotificationChannel(channelId, "General", NotificationImportance.Max);
            channel.EnableVibration(true);
            notificationManager.CreateNotificationChannel(channel);
            FirebaseCloudMessagingImplementation.ChannelId = channelId;
            FirebaseCloudMessagingImplementation.SmallIconRef = Resource.Drawable.pushIcon;

            //var channelId2 = $"{PackageName}.Notifications";
            //var notificationManager2 = (NotificationManager)GetSystemService(NotificationService);
            //var channel2 = new NotificationChannel(channelId2, "Notifications", NotificationImportance.Max);
            //channel.EnableVibration(true);
            //notificationManager.CreateNotificationChannel(channel2);
            //FirebaseCloudMessagingImplementation.ChannelId = channelId2;
            //FirebaseCloudMessagingImplementation.SmallIconRef = Resource.Drawable.pushIcon;
        }

        public override void OnBackPressed()
        {
            if (App.Current.MainPage != null && App.Current.MainPage.Navigation.NavigationStack.Last().GetType().Name == "MainPage")
            {
                if (Pages.Index.IsBackDisabled())
                {
                   // Pages.Index.ReEnableBack();
                    //ReEnableBack() does not close the popups, why?
                    return;
                }
            }
            base.OnBackPressedDispatcher.OnBackPressed();
        }

    }
}
//        protected override void OnCreate(Bundle savedInstanceState)
//        {
//            base.OnCreate(savedInstanceState);
//            CreateNotificationChannel();
//            HandleIntent(Intent);
//        }

//        protected async override void OnNewIntent(Intent intent)
//        {
//            base.OnNewIntent(intent);
//            if (intent.Extras != null)
//            {
//                foreach (var key in Intent.Extras.KeySet())
//                {
//                    var value = Intent.Extras.GetString(key);
//                    if (value == "CHAT")
//                    {
//                        await MainThread.InvokeOnMainThreadAsync(async () => { await Microsoft.Maui.Controls.Application.Current.MainPage.Navigation.PushAsync(new ViewHybridChatContentPage()); });
//                    }
//                }
//            }
//        }

//        private async static void HandleIntent(Intent intent)
//        {
//            if (intent.Extras != null)
//            {
//                foreach (var key in intent.Extras.KeySet())
//                {
//                    var value = intent.Extras.GetString(key);

//                    if (value == "CHAT")
//                    {
//                        await MainThread.InvokeOnMainThreadAsync(async () => { await Microsoft.Maui.Controls.Application.Current.MainPage.Navigation.PushAsync(new ViewHybridChatContentPage()); });
//                    }

//                }
//            }

//        //    FirebaseCloudMessagingImplementation.OnNewIntent(intent);
//        }

//        private void CreateNotificationChannel()
//        {
//            var channelId = $"{PackageName}.General";
//            var notificationManager = (Android.App.NotificationManager)GetSystemService(NotificationService);
//            var channel = new NotificationChannel(channelId, "General", NotificationImportance.Max);
//            channel.EnableVibration(true);
//            notificationManager.CreateNotificationChannel(channel);
//            FirebaseCloudMessagingImplementation.ChannelId = channelId;
//            FirebaseCloudMessagingImplementation.SmallIconRef = Resource.Drawable.pushIcon;
//        }


//        public override void OnBackPressed()
//        {
//            //base.OnBackPressed();
//        }
//    }



//    [Service(Exported = true)]
//    [IntentFilter(new[] { IntentAction })]
//    public class ShinyFirebaseService2 : FirebaseMessagingService
//    {
//        public const string IntentAction = "com.google.firebase.MESSAGING_EVENT";

//        internal static Action<RemoteMessage>? MessageReceived { get; set; }

//        override ON

//        public async override void OnMessageReceived(RemoteMessage message)
//        {
//            base.OnMessageReceived(message);


//            sendMyNotification(message);

//            foreach (var k in message.Data)
//            {
//                string m = k.Key;
//                string l = k.Value;
//                if (m == "click_action" && l == "CHAT")
//                {
//                   // await MainThread.InvokeOnMainThreadAsync(async () => { await Microsoft.Maui.Controls.Application.Current.MainPage.Navigation.PushAsync(new ViewHybridChatContentPage()); });
               
                
//                }
//            }

//        }

//        public override void OnMessageSent(string msgId)
//        {
//            base.OnMessageSent(msgId);
//        }


//        internal static Action<string>? NewToken { get; set; }
//        public override void OnNewToken(string token)
//        {
//            NewToken?.Invoke(token);
//        }



//        private void sendMyNotification(RemoteMessage remoteMessage)
//        {

//            //var notificationManager = (NotificationManager)GetSystemService(NotificationService);
//            //var stackBuilder = AndroidX.Core.App.TaskStackBuilder.Create(this);
//            //var resultPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);


//            //var resultIntent = new Android.Content.Intent(this, typeof(MainActivity));
//            //var valuesForActivity = new Bundle();
//            //valuesForActivity.PutString("ACTION", "CHAT");

//            //// Pass some values to SecondActivity:
//            //resultIntent.PutExtras(valuesForActivity);



//            // Pass the current button press count value to the next activity:
//            var valuesForActivity = new Bundle();
//            valuesForActivity.PutString("ACTION", "CHAT");

//            // When the user clicks the notification, SecondActivity will start up.
//            var resultIntent = new Intent(this, typeof(MainActivity));

//            // Pass some values to SecondActivity:
//            resultIntent.PutExtras(valuesForActivity);

//            // Construct a back stack for cross-task navigation:
//            var stackBuilder = AndroidX.Core.App.TaskStackBuilder.Create(this);
//          //  stackBuilder.AddParentStack(Class.FromType(typeof(MainActivity)));
//            stackBuilder.AddNextIntent(resultIntent);

//            var resultPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.Immutable);


//            FirebaseMessaging.Instance..onMessageOpenedApp.listen((RemoteMessage message) async {

//                if (message != null)
//                {
//                    String screenName = message.data['screenName'];
//                    print("Screen name is: $screenName");

//                    if (screenName == 'chat')
//                    {
//                        //Navigate to your chat screen here
//                    }
//                }
//            });


//            // Build the notification:
//            var builder = new NotificationCompat.Builder(this, "3")
//                          .SetAutoCancel(true) // Dismiss the notification from the notification area when the user clicks on it
//                          .SetContentIntent(resultPendingIntent) // Start up this activity when the user clicks the intent.
//                          .SetContentTitle("Button Clicked") // Set the title
//                          .SetNumber(3) // Display the count in the Content Info
//                          .SetSmallIcon(Resource.Drawable.pushIcon) // This is the icon to display
//                          .SetContentText($"The button has been clicked  times."); // the message to display.

//            // Finally, publish the notification:
//            var notificationManager2 = NotificationManagerCompat.From(this);
//            notificationManager2.Notify(1001, builder.Build());

//        }

//        void CreateChannel(INotificationManager nofif, string name, ChannelImportance imp) => nofif.AddChannel(new Channel
//        {
//            Identifier = name,
//            Description = name,
//            Importance = imp,
//            // Sound = ChannelSound.High
//        });
//    }


//    //        protected override void OnCreate(Bundle savedInstanceState)
//    //        {
//    //            base.OnCreate(savedInstanceState);
//    //         //   HandleIntent(Intent);
//    //            CreateNotificationChannelIfNeeded();
//    //          //  checkIntent(Intent);
//    //        }

//    //        protected override void OnNewIntent(Android.Content.Intent intent2)
//    //        {
//    //            base.OnNewIntent(intent2);

//    //          //  checkIntent(intent2);

//    //            if (intent2.Extras != null)
//    //            {
//    //                foreach(var m in intent2.Categories)
//    //                {
//    //                    var jj = m;
//    //                    bool stop = true;
//    //                }
//    //                foreach (var key in intent2.Extras.KeySet())
//    //                {
//    //                    var value = intent2.Extras.GetString(key);

//    //                    if (key == "webContentList") // Make it Dynamic instead of hardcoding here 
//    //                    {
//    //                        if (value?.Length > 0)
//    //                        {

//    //                        }
//    //                    }
//    //                }
//    //            }


//    //        //    HandleIntent(intent2);

//    //        }

//    //        //public void checkIntent(Android.Content.Intent intent2)
//    //        //{
//    //        //    String s = new String("ViewHybridChatContentPage");
//    //        //    ClickActionHelper.startActivity(s, intent2.GetBundleExtra(""), this);
//    //        //    if (intent2.Extras != null)
//    //        //    {
//    //        //        foreach (var m in intent2.Categories)
//    //        //        {
//    //        //            var jj = m;
//    //        //            bool stop = true;
//    //        //        }
//    //        //        foreach (var key in intent2.Extras.KeySet())
//    //        //        {
//    //        //            var value = intent2.Extras.GetString(key);

//    //        //            if (key == "webContentList") // Make it Dynamic instead of hardcoding here 
//    //        //            {
//    //        //                if (value?.Length > 0)
//    //        //                {

//    //        //                }
//    //        //            }
//    //        //        }
//    //        //    }

//    //        //    if (intent2.HasExtra("click_action"))
//    //        //    {
//    //        //        ClickActionHelper.startActivity((String)intent2.GetStringExtra("click_action"), intent2.GetBundleExtra(""), this);
//    //        //    }
//    //        //}

//    //            private static void HandleIntent(Intent intent)
//    //        {


//    //        //    FirebaseCloudMessagingImplementation.OnNewIntent(intent);
//    //        }

//    //        private void CreateNotificationChannelIfNeeded()
//    //        {
//    //            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
//    //            {
//    //                CreateNotificationChannel();
//    //            }
//    //        }

//    //        private void CreateNotificationChannel()
//    //        {
//    //            var channelId = $"{PackageName}.General";
//    //            var notificationManager = (Android.App.NotificationManager)GetSystemService(NotificationService);
//    //            var channel = new NotificationChannel(channelId, "General", NotificationImportance.Max);
//    //            channel.EnableVibration(true);
//    //            notificationManager.CreateNotificationChannel(channel);
//    //            FirebaseCloudMessagingImplementation.ChannelId = channelId;
//    //            FirebaseCloudMessagingImplementation.SmallIconRef = Resource.Drawable.pushIcon;
//    //        }
//    //    }


//    //    [Service(Exported = true)]
//    //    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
//    //    public class MyFirebaseMessagingService : FirebaseMessagingService
//    //    {
//    //        // private string TAG = "MyFirebaseMsgService";
//    //        public override void OnMessageReceived(RemoteMessage message)
//    //        {
//    //            base.OnMessageReceived(message);
//    //            string messageFrom = message.From;
//    //            string getMessageBody = message.GetNotification().Body;
//    //            SendNotification(message.GetNotification().Body);
//    //        }
//    //        void SendNotification(string messageBody)
//    //        {
//    //            try
//    //            {
//    //                var notif = Host.Current.Services.GetService<INotificationManager>();

//    //                CreateChannel(notif, "3", ChannelImportance.High);

//    //                this.CreateNotification(notif, 3, "3", "TTITLE", "BBODY", 1);

//    //            }
//    //            catch (Exception ex)
//    //            {

//    //            }
//    //        }

//    //        void CreateNotification(INotificationManager nofif, int id, string channel, string Title, string Message, int badege)
//    //        {
//    //            Dictionary<string, string> extrax = new Dictionary<string, string>();
//    //            extrax.Add("click_action", "ViewHybridChatContentPage");
//    //            nofif.Send(new Shiny.Notifications.AndroidNotification
//    //            {
//    //                Id = id,
//    //                //  UseBigTextStyle = true,
//    //                LaunchActivityType = typeof(Activity),
//    //                Title = Title,
//    //                Message = Message,
//    //                Channel = channel,
//    //                BadgeCount = badege,
//    //                Payload = extrax,
//    //            });
//    //        }

//    //        void CreateChannel(INotificationManager nofif, string name, ChannelImportance imp) => nofif.AddChannel(new Channel
//    //        {
//    //            Identifier = name,
//    //            Description = name,
//    //            Importance = imp,
//    //            // Sound = ChannelSound.High
//    //        });
//    //    }



//    //    //public class ClickActionHelper
//    //    //{
//    //    //    public static void startActivity(String className, Bundle extras, Context context)
//    //    //    {
//    //    //        Intent intent = new Intent()
//    //    //        intent.
//    //    //        Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.StartActivity(intent);
//    //    //        Class cls;
//    //    //        string s = (string)className;
//    //    //        try
//    //    //        {
//    //    //            cls = Class.ForName(s);
//    //    //            Intent i = new Intent(context, cls);
//    //    //            i.PutExtras(extras);
//    //    //            context.StartActivity(i);
//    //    //        }
//    //    //        catch (ClassNotFoundException e)
//    //    //        {
//    //    //            //means you made a wrong input in firebase console
//    //    //        }

//    //    //    }
//    //    //}

//    //[Activity(Label = "SecondActivity", Exported = true, Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density | ConfigChanges.Orientation, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
//    //[IntentFilter(new[] { ".Activities.SecondActivity" }, Categories = new[] { "android.intent.category.DEFAULT" })]
//    //public class SecondActivity : MauiAppCompatActivity
//    //{
//    //    protected async override void OnCreate(Bundle savedInstanceState)
//    //    {
//    //        base.OnCreate(savedInstanceState);

//    //        await MainThread.InvokeOnMainThreadAsync(async () => { await Microsoft.Maui.Controls.Application.Current.MainPage.Navigation.PushAsync(new ViewHybridChatContentPage()); });

//    //        //   HandleIntent(Intent);




//    //        // CreateNotificationChannelIfNeeded();
//    //    }

//    //    //private static void HandleIntent(Intent intent)
//    //    //{
//    //    //    FirebaseCloudMessagingImplementation.OnNewIntent(intent);
//    //    //}


//    //    private void CreateNotificationChannelIfNeeded()
//    //    {
//    //        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
//    //        {
//    //            CreateNotificationChannel();
//    //        }
//    //    }

//    //    private void CreateNotificationChannel()
//    //    {
//    //        var channelId = $"{PackageName}.General";
//    //        var notificationManager = (Android.App.NotificationManager)GetSystemService(NotificationService);
//    //        var channel = new NotificationChannel(channelId, "General", NotificationImportance.Max);
//    //        channel.EnableVibration(true);
//    //        notificationManager.CreateNotificationChannel(channel);
//    //        FirebaseCloudMessagingImplementation.ChannelId = channelId;
//    //        FirebaseCloudMessagingImplementation.SmallIconRef = Resource.Drawable.pushIcon;
//    //    }
//    //}
//}
////}