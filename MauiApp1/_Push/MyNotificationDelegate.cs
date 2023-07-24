using MauiApp1._Push;
using Shiny.Notifications;

namespace Sample.Notifications;


public class MyNotificationDelegate : INotificationDelegate
{
   // readonly SampleSqliteConnection conn;
    readonly INotificationManager notifications;


    public MyNotificationDelegate(INotificationManager notifications)
    {
     //   this.conn = conn;
        this.notifications = notifications;
    }


    public  Task OnEntry(NotificationResponse response)
    {
#if IOS
        try
        {
            var l = response.Notification;
            string action = "";
            string param1 = "";
            if (l.Payload.Keys.Contains("aaction"))
            {
                 action = l.Payload.Where(t => t.Key == "aaction").FirstOrDefault().Value;
            }
            if (l.Payload.Keys.Contains("param1"))
            {
                param1 = l.Payload.Where(t => t.Key == "param1").FirstOrDefault().Value;
            }
      
            if(!string.IsNullOrEmpty(action))
            {
                PushNavigationHelper.HandleNotificationTab(action, param1);
            }
        }
        catch (Exception ex)
        {
            return Task.FromResult<object>(null);
        }
#endif
              return Task.FromResult<object>(null);
    }

    public  Task OnReceived(NotificationResponse response)
    {
        return Task.FromResult<object>(null);
    }
}
