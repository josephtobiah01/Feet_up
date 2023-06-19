using Foundation;
using Shiny.Hosting;
using UIKit;
using UserNotifications;

namespace MauiApp1
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        [Export("application:didRegisterForRemoteNotificationsWithDeviceToken:")]
        public void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
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
             Host.Lifecycle.OnDidReceiveRemoteNotification(userInfo, completionHandler);
        }

        [Export("userNotificationCenter:willPresent:withCompletionHandler:")]
        public void DidReceiveRemoteNotification2(UNUserNotificationCenter _UNUserNotificationCenter, UNNotification notif, UNNotificationPresentationOptions completionHandler)
        {
            var stop = true;
        }


        [Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
        public void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            var stop = true;
        }


        public override void WillEnterForeground(UIApplication uiApplication)
        {
            var stop = true;
            //Plugin.LocalNotification.NotificationCenter.ResetApplicationIconBadgeNumber(uiApplication);
        }
    }
}