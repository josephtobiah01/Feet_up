using Foundation;
using Shiny.Hosting;
using UIKit;

namespace MauiApp1
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    //    [Export("application:didRegisterForRemoteNotificationsWithDeviceToken:")]
    //    public void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
    //=> Host.Lifecycle.OnRegisteredForRemoteNotifications(deviceToken);

        //[Export("application:didFailToRegisterForRemoteNotificationsWithError:")]
        //public void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        //    => Host.Lifecycle.OnFailedToRegisterForRemoteNotifications(error);

        [Export("application:didReceiveRemoteNotification:fetchCompletionHandler:")]
        public void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
            => Host.Lifecycle.OnDidReceiveRemoveNotification(userInfo, completionHandler);


        [Foundation.Export("application:didRegisterForRemoteNotificationsWithDeviceToken:")]
        public virtual void RegisteredForRemoteNotifications(UIKit.UIApplication application, NSData deviceToken)
        {
            int x = 0;
        }

        [Export("application:didFailToRegisterForRemoteNotificationsWithError:")]
        public void FailedToRegisterForRemoteNotifications(UIKit.UIApplication application, NSError error)
        {
            int x = 0;
        }

    }
}