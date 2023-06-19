#if ANDROID
using Firebase;
using ParentMiddleWare;
using Plugin.FirebasePushNotification;
#endif
using ParentMiddleWare;
using Shiny.Hosting;
using Shiny.Push;

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

            if (MiddleWare.UserID <= 0)
            {
                return null;
            }
#if ANDROID
                var k =  CrossFirebasePushNotification.Current.Token;

            //if (!isInitialized)
            //{
            //    isInitialized = true;

            //    var options = new Fireb aseOptions.Builder()
            //         .SetApplicationId(PushRegistration.AppId)
            //         .SetProjectId(PushRegistration.ProjectId)
            //         .SetApiKey(PushRegistration.ApiKey)
            //         .SetGcmSenderId(PushRegistration.SenderId)
            //    .Build();

            //    FirebaseApp.InitializeApp(Android.App.Application.Context, options);
            //}

            var push = Host.Current.Services.GetService<IPushManager>();

            if (!string.IsNullOrEmpty(push.RegistrationToken))
            {
                return push.RegistrationToken;
            }


            var result = await push.RequestAccess();
            return result.RegistrationToken;



#endif
#if IOS
            var push = Host.Current.Services.GetService<IApplePushManager>();
            if (!string.IsNullOrEmpty(push.RegistrationToken))
            {
                return push.RegistrationToken;
            }

            var result = await push.RequestAccess(UserNotifications.UNAuthorizationOptions.ProvidesAppNotificationSettings |  UserNotifications.UNAuthorizationOptions.Badge | UserNotifications.UNAuthorizationOptions.Sound | UserNotifications.UNAuthorizationOptions.Alert);

            if (result.Status == AccessState.Available)
            {

                // good to go
                // var token1 = service.NativeToken;
                //  var token2 = service.InstallationId;
                // you should send this to your server with a userId attached if you want to do custom work

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