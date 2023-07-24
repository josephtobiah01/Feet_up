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
            //UNUserNotificationCenter.Current.RemoveAllPendingNotificationRequests();
            //UNUserNotificationCenter.Current.RemoveAllDeliveredNotifications();
            blockNotifications = true;
            BloclTime = DateTime.Now;
            Host.Lifecycle.OnRegisteredForRemoteNotifications(deviceToken);
        }


        [Export("application:didFailToRegisterForRemoteNotificationsWithError:")]
        public void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            Host.Lifecycle.OnFailedToRegisterForRemoteNotifications(error);
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
                    Host.Lifecycle.OnDidReceiveRemoveNotification(userInfo, completionHandler);
                }
            }
            catch(Exception e)
            {

            }
            //   Host.Lifecycle.OnDidReceiveRemoteNotification(userInfo, completionHandler);
         //   Host.Lifecycle.OnDidReceiveRemoveNotification(userInfo, completionHandler);
        }

        //[Export("userNotificationCenter:willPresent:withCompletionHandler:")]
        //public void DidReceiveRemoteNotification2(UNUserNotificationCenter _UNUserNotificationCenter, UNNotification notif, UNNotificationPresentationOptions completionHandler)
        //{
        //    var stop = true;
        //}


        //[Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
        //public void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        //{
        //    var stop = true;
        //}



        public override void WillEnterForeground(UIApplication uiApplication)
        {

            blockNotifications = true;
            BloclTime = DateTime.Now;
           //UNUserNotificationCenter.Current.RemoveAllPendingNotificationRequests();
           //UNUserNotificationCenter.Current.RemoveAllDeliveredNotifications();

           ////      UIApplication.n
           //  UIApplication.SharedApplication.CancelAllLocalNotifications();
           var stop = true;
         //   Plugin.LocalNotification.NotificationCenter.ResetApplicationIconBadgeNumber(uiApplication);
        }
    }
}