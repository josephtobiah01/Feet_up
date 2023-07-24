using Android.App;
using Android.Content.PM;
using Android.OS;
//using Plugin.Firebase.CloudMessaging;
using Plugin.FirebasePushNotification;
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
            try
            {

                FirebasePushNotificationManager.ProcessIntent(this, Intent, true);
            }
            catch { }
            //HandleIntent(Intent);
            // CreateNotificationChannelIfNeeded();
        }

        protected override void OnNewIntent(Intent intent)
        {
            try
            {
                base.OnNewIntent(intent);
            FirebasePushNotificationManager.ProcessIntent(this, intent, true);
                //   HandleIntent(intent);
            }
            catch { }
        }


        private static void HandleIntent(Intent intent)
        {
         //   FirebaseCloudMessagingImplementation.OnNewIntent(intent);
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
          //  var channelId = $"{PackageName}.General";
          //  var notificationManager = (NotificationManager)GetSystemService(NotificationService);
        //    var channel = new NotificationChannel(channelId, "General", NotificationImportance.Max);
          //  channel.EnableVibration(true);
           // notificationManager.CreateNotificationChannel(channel);
           
            
            
            //FirebaseCloudMessagingImplementation.ChannelId = channelId;
            //FirebaseCloudMessagingImplementation.SmallIconRef = Resource.Drawable.pushIcon;

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
            //base.OnBackPressedDispatcher.OnBackPressed();
            base.OnBackPressed();
        }

    }
}
