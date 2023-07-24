using static Microsoft.Maui.ApplicationModel.Permissions;

namespace MauiApp1.Helpers;

public class PushNotificationPermissionsAndroid : BasePlatformPermission
{
#if ANDROID
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
                new List<(string permission, bool isruntime)>
                {
                    ("android.permission.POST_NOTIFICATIONS", true)
                }.ToArray();
#endif
}

