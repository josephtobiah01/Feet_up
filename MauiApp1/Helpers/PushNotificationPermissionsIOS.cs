using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if IOS

using UserNotifications;

#endif

namespace MauiApp1.Helpers;
public class PushNotificationPermissionsIOS
{
    public async Task<bool> CheckAsync()
    {
#if IOS
        var k = UNUserNotificationCenter.Current;
        var status  = await k.GetNotificationSettingsAsync();
        if (status.AuthorizationStatus == UNAuthorizationStatus.Authorized)  
        {
            return true;
        }
        else
        {
            return false;
        }
#endif
        return false;
    }
    public async Task RequestAsync()
    {
#if IOS
        var k = UNUserNotificationCenter.Current;
        await k.RequestAuthorizationAsync(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound);
#endif
    }

}