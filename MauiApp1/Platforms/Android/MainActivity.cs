using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
//using Plugin.Firebase.CloudMessaging;
using Plugin.FirebasePushNotification;
using Intent = Android.Content.Intent;

namespace MauiApp1
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    //[Activity(
    //    Theme = "@style/Maui.SplashTheme",
    //    MainLauncher = true, 
    //    ConfigurationChanges = ConfigChanges.ScreenSize 
    //    | ConfigChanges.UiMode 
    //    | ConfigChanges.ScreenLayout 
    //    | ConfigChanges.SmallestScreenSize
    //    | ConfigChanges.Density
    //    | ConfigChanges.Orientation
    //    )]

    public class MainActivity : MauiAppCompatActivity
    {

        public static Context context;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //try
            //{
                base.OnCreate(savedInstanceState);
                NativeMedia.Platform.Init(this, savedInstanceState);
                //  FirebasePushNotificationManager.ProcessIntent(this, Intent, true);
            //}
            //catch(Exception e)
            //{ 
            
            //}
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent intent)
        {
            if (NativeMedia.Platform.CheckCanProcessResult(requestCode, resultCode, intent))
                NativeMedia.Platform.OnActivityResult(requestCode, resultCode, intent);

            base.OnActivityResult(requestCode, resultCode, intent);
        }

        public override void OnLowMemory()
        {
            base.OnLowMemory();
        }

        protected override void OnStop()
        {
            base.OnStop();
        }


        protected override void OnRestart()
        {
            base.OnRestart();
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();

                //    FirebasePushNotificationManager.ProcessIntent(this, Intent, true);
                FirebasePushNotificationManager.ProcessIntent(this, Intent);
            }
            catch { }
        }

        protected override void OnNewIntent(Intent intent)
        {
            try
            {
                base.OnNewIntent(intent);
                FirebasePushNotificationManager.ProcessIntent(this, intent);
            }
            catch { }
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
