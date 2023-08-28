#if ANDROID
using Firebase;
using ParentMiddleWare;
using Plugin.FirebasePushNotification;
#endif
#if IOS
using Shiny.Hosting;
using Shiny.Push;
using Shiny.Notifications;
#endif
using ParentMiddleWare;

public class PushRegistration
{

    private static string AppId = "1:368008538066:android:b25aacfaa968fe44130a0b";
    private static string SenderId = "368008538066";
    private static string ProjectId = "age-in-reverse-longevity";
    private static string ApiKey = "AIzaSyAkP7jzgygDHerCG7PVU5Lo3l-nhbLYrLk";


    public static bool isInitialized = false;
    public static async Task<string> CheckPermission()
    {
        try
        {        
            if (MiddleWare.FkFederatedUser == string.Empty)
            {
                return null;
            }
#if ANDROID

            try
            {
                var k = CrossFirebasePushNotification.Current.Token;
                return k;
            }
            catch { }
#endif
#if IOS
            var push = Host.Current.Services.GetService<IApplePushManager>();
            if (!string.IsNullOrEmpty(push.RegistrationToken))
            {
                return push.RegistrationToken;
                //var list = push.Tags;
                //var kkk = list.GetValue("InstallationId");
                //return kkk.ToString(); ;
            }

            var result = await push.RequestAccess(UserNotifications.UNAuthorizationOptions.ProvidesAppNotificationSettings | UserNotifications.UNAuthorizationOptions.Badge | UserNotifications.UNAuthorizationOptions.Sound | UserNotifications.UNAuthorizationOptions.Alert);

            if (result.Status == AccessState.Available)
            {

                // good to go
                // var token1 = service.NativeToken;
                //  var token2 = service.InstallationId;
                // you should send this to your server with a userId attached if you want to do custom work
                //var list = push.Tags;
                // var kkk = list.GetValue("InstallationId");
                //return kkk.ToString();
     

           //     var apush = Host.Current.Services.GetService<AzureNotificationHubsPushProvider>();
            


                return result.RegistrationToken;
            }
#endif
            return null;
        }
        catch (Exception e)
        {
            return null;
        }

    }



    public static string GetPlatform()
    {
        var platform = DeviceInfo.Current.Platform;

        if (platform == DevicePlatform.Android) return "D";
        else if (platform == DevicePlatform.iOS) return "A";
        else return "U";

    }
}