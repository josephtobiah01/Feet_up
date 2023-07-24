using MauiApp1._Push;
using MauiApp1.Areas.Chat.Views;
using Shiny.Notifications;
using Shiny.Push;
#if IOS
using UIKit;
#endif
namespace Sample;


public class NewPushDelegate : IPushDelegate
{

    readonly IPushManager pushManager;
    readonly INotificationManager _notificationManager;

    public NewPushDelegate(IPushManager pushManager, INotificationManager notificationManager)
    {
        // this.conn = conn;
        this.pushManager = pushManager;
        _notificationManager = notificationManager;
    }


    public Task OnReceived(IDictionary<string, string> data)
    {
        return Task.FromResult<object>(null);
    }

    public Task OnTokenChanged(string token)
    {
        return Task.FromResult<object>(null);
    }

    public Task OnEntry(PushNotification push)
    {

        bool stop = true;
#if IOS
        try
        {
            var apnstring = push.data.Values.FirstOrDefault();
            //string title = parseAlert(apnstring);
            //string body = parseBody(apnstring);
            //   int badge = parseBadge(apnstring);
            string action = parseAction(apnstring);
            string param = parseParam(apnstring);

            if (!String.IsNullOrEmpty(action))
            {
                    PushNavigationHelper.HandleNotificationTab(action, param);
            }
        }
        catch (Exception ex)
        {
            return Task.FromResult<object>(null);
        }
#endif
        return Task.FromResult<object>(null);
    }


    public Task OnReceived(PushNotification push)
    {
        try
        {
#if IOS
            if (UIApplication.SharedApplication.ApplicationState != UIApplicationState.Active) return Task.FromResult<object>(null); ;
            var apnstring = push.data.Values.FirstOrDefault();
            string title = parseAlert(apnstring);
            string body = parseBody(apnstring);
            int badge = parseBadge(apnstring);
            string action = parseAction(apnstring);
            string param1 = parseParam(apnstring);

            // CREATE CHANNEL?

            //    this.CreateChannel("3", ChannelImportance.High);

            Dictionary<string, string> pl = new Dictionary<string, string>();
            pl.Add("aaction", action);
            pl.Add("param1", param1);


            this.CreateNotification(3, "Notifications", title, body, 0, pl);
#endif
        }
        catch (Exception ex)
        {
            return Task.FromResult<object>(null);
        }
        return Task.FromResult<object>(null);
    }




    //public Task OnEntry(PushNotification push)
    //=> this.Insert("PUSH ENTRY");

    //public Task OnReceived(PushNotification push)
    //    => this.Insert("PUSH RECEIVED");

    public Task OnTokenRefreshed(string token)
        => this.Insert("PUSH TOKEN REFRESH");

    async Task Insert(string info) => await Task.Delay(1);





#if IOS
    Task CreateNotification(int id, string channel, string Title, string Message, int badege, Dictionary<string, string> payload)
    => this._notificationManager.Send(new Shiny.Notifications.AppleNotification
    {
        Id = id,
        Title = Title,
        Message = Message,
        Channel = channel,
        BadgeCount = badege,
        Payload = payload,
    });
#endif

    //void CreateChannel(string name, ChannelImportance imp) => this._notificationManager.AddChannel(new Channel
    //{
    //    Identifier = name,
    //    Description = name,
    //    Importance = imp,
    //   // Sound = ChannelSound.High
    //});



    static string parseAlert(string y)
    {
        try
        {
            var ai = y.IndexOf("title");
            if (ai < 0) return null;
            var fi = y.IndexOf("=", ai + 1);
            var ei = y.IndexOf(";", fi + 1);
            y = y.Substring(fi + 1, ei - fi - 1).Trim();
            y = y.Trim(new char[] { '\\', '"' });
            y = y.Trim(new char[] { '"', '\\' });
            return y.Trim();
        }
        catch
        {
            return null;
        }
    }

    static string parseAction(string y)
    {
        try
        {
            var ai = y.IndexOf("aaction");
            if (ai < 0) return null;
            var fi = y.IndexOf("=", ai + 1);
            var ei = y.IndexOf(";", fi + 1);
            y = y.Substring(fi + 1, ei - fi - 1).Trim();
            y = y.Trim(new char[] { '\\', '"' });
            y = y.Trim(new char[] { '"', '\\' });
            return y.Trim();
        }
        catch
        {
            return null;
        }
    }

    static string parseParam(string y)
    {
        try
        {
            var ai = y.IndexOf("param1");
            if (ai < 0) return null;
            var fi = y.IndexOf("=", ai + 1);
            var ei = y.IndexOf(";", fi + 1);
            y = y.Substring(fi + 1, ei - fi - 1).Trim();
            y = y.Trim(new char[] { '\\', '"' });
            y = y.Trim(new char[] { '"', '\\' });
            return y.Trim();
        }
        catch
        {
            return "";
        }
    }

    static string parseBody(string y)
    {
        try
        {
            var ai = y.IndexOf("body");
            if (ai < 0) return null;
            var fi = y.IndexOf("=", ai + 1);
            var ei = y.IndexOf(";", fi + 1);
            y = y.Substring(fi + 1, ei - fi - 1).Trim();
            y = y.Trim(new char[] { '\\', '"' });
            y = y.Trim(new char[] { '"', '\\' });
            return y.Trim();
        }
        catch
        {
            return null;
        }
    }

    static string parseData(string y)
    {
        try
        {
            var ai = y.IndexOf("subtitle");
            if (ai < 0) return null;
            var fi = y.IndexOf("\"", ai + 1);
            var ei = y.IndexOf("\"", fi + 1);
            y = y.Substring(fi + 1, ei - fi - 1).Trim();
            y = y.Trim(new char[] { '\\', '"' });
            y = y.Trim(new char[] { '"', '\\' });
            return y.Trim();
        }
        catch
        {
            return null;
        }
    }

    static int parseBadge(string y)
    {
        try
        {
            var ai = y.IndexOf("badge");
            if (ai < 0) return 0;
            var fi = y.IndexOf("=", ai + 1);
            var ei = y.IndexOf(";", fi + 1);
            y = y.Substring(fi + 1, ei - fi - 1).Trim();
            y = y.Trim(new char[] { '\\', '"' });
            y = y.Trim(new char[] { '"', '\\' });
            return int.Parse(y.Trim());
        }
        catch
        {
            return 0;
        }
    }
}


//using Shiny.Push;

//namespace Sample;


//public class MyPushDelegate : IPushDelegate
//{
//   // readonly SampleSqliteConnection conn;
//    readonly IPushManager pushManager;


//    public MyPushDelegate( IPushManager pushManager)
//    {
//        this.pushManager = pushManager;
//    }

//    public Task OnEntry(PushNotification push)
//        => this.Insert("PUSH ENTRY");

//    public Task OnReceived(PushNotification push)
//        => this.Insert("PUSH RECEIVED");

//    public Task OnTokenRefreshed(string token)
//        => this.Insert("PUSH TOKEN REFRESH");

//    Task Insert(string info) 
//    {
//        return null;
//    }
//}
