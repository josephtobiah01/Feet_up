//using Foundation;
//using UIKit;
//using Shiny.Hosting;


//namespace MauiApp1
//{
//    [Register("AppDelegate")]
//    public class AppDelegate : MauiUIApplicationDelegate
//    {
//        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

//        [Export("application:didRegisterForRemoteNotificationsWithDeviceToken:")]
//        public void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
//            => Host.Lifecycle.OnRegisteredForRemoteNotifications(deviceToken);

//        [Export("application:didFailToRegisterForRemoteNotificationsWithError:")]
//        public void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
//            => Host.Lifecycle.OnFailedToRegisterForRemoteNotifications(error);

//        [Export("application:didReceiveRemoteNotification:fetchCompletionHandler:")]
//        public void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
//            => Host.Lifecycle.OnDidReceiveRemoteNotification(userInfo, completionHandler);
//    }
//}



using Foundation;
using Shiny.Hosting;
using UIKit;
using UserNotifications;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MauiApp1
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        public static bool blockNotifications = false;
        public static DateTime BloclTime = DateTime.Now;

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        [Export("application:didRegisterForRemoteNotificationsWithDeviceToken:")]
        public void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            try
            {
                blockNotifications = true;
                BloclTime = DateTime.Now;
                Host.Lifecycle.OnRegisteredForRemoteNotifications(deviceToken);
            }
            catch (Exception ex)
            {

            }
        }


        [Export("application:didFailToRegisterForRemoteNotificationsWithError:")]
        public void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            try
            {
                Host.Lifecycle.OnFailedToRegisterForRemoteNotifications(error);
            }
            catch (Exception ex)
            {

            }
        }


        [Export("application:didReceiveRemoteNotification:fetchCompletionHandler:")]
        public void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            try
            {
                if (blockNotifications && DateTime.Now.Subtract(BloclTime).TotalSeconds > 6)
                {
                    blockNotifications = false;
                }
                if (!blockNotifications)
                {
                  // Host.Lifecycle.OnDidReceiveRemoteNotification(userInfo, completionHandler);
                     Host.Lifecycle.OnDidReceiveRemoveNotification(userInfo, completionHandler);
                }
            }
            catch (Exception e)
            {

            }
        }

        public override void WillEnterForeground(UIApplication uiApplication)
        {
            try
            {
                blockNotifications = true;
                BloclTime = DateTime.Now;
            }
            catch (Exception ex)
            {

            }
        }
    }
}