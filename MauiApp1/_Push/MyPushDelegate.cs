using MauiApp1.Areas.Chat.Views;
using Shiny.Notifications;
using Shiny.Push;

namespace Sample;


public class MyPushDelegate : IPushDelegate
{

    readonly IPushManager pushManager;
    readonly INotificationManager _notificationManager;

    public MyPushDelegate(IPushManager pushManager, INotificationManager notificationManager)
    {
        // this.conn = conn;
        this.pushManager = pushManager;
        _notificationManager = notificationManager;
    }

    //public async Task OnEntry(PushEntryArgs args)
    //{
    //    // fires when the user taps on a push notification
    //}

    public async Task OnReceived(IDictionary<string, string> data)
    {
        await Application.Current.MainPage.Navigation.PushAsync(new ViewHybridChatContentPage());
        var x = false;
    }

    public async Task OnTokenChanged(string token)
    {
        // fires when a push notification change is set by the operating system or provider
    }

    public async Task OnEntry(PushNotification push)
    {
        bool stop = true;
#if IOS
        var apnstring = push.data.Values.FirstOrDefault();
        string title = parseAlert(apnstring);
        string body = parseBody(apnstring);
        int badge = parseBadge(apnstring);
        string action = parseAction(apnstring);

        //Console.WriteLine(title);
        //Console.WriteLine(body);
        //Console.WriteLine(badge);
        //Console.WriteLine(asction);

        if (action == "CHAT")
        {
            await Application.Current.MainPage.Navigation.PushAsync(new ViewHybridChatContentPage());
        }
#endif

        
    }

    public async Task OnReceived(PushNotification push)
    {
        bool stop = true;
        try
        {
#if IOS

            var apnstring = push.data.Values.FirstOrDefault();
            string title = parseAlert(apnstring);
            string body = parseBody(apnstring);
            int badge = parseBadge(apnstring);
            string action = parseAction(apnstring);


            // CREATE CHANNEL?
            
            this.CreateChannel("3", ChannelImportance.Critical);

            Dictionary<string, string> pl = new Dictionary<string, string>();
            pl.Add("aaction", action);

            await this.CreateNotification(3, "3", title, body, badge, pl);

            //var options = new SnackbarOptions
            //{
            //    BackgroundColor = Colors.Red,
            //    TextColor = Colors.Green,
            //    ActionButtonTextColor = Colors.Yellow,
            //    CornerRadius = new CornerRadius(10),
            //    Font = Microsoft.Maui.Font.SystemFontOfSize(14),
            //    ActionButtonFont = Microsoft.Maui.Font.SystemFontOfSize(14),
            //    CharacterSpacing = 0.5 
            //};

            //if (action == "CHAT")
            //{
            //    Action action1 = new Action(async () => { await Application.Current.MainPage.Navigation.PushAsync(new ViewIOSChatContentPage()); });
            //    var snackbar = Snackbar.Make("You Have a new Chatmessage", action1, "View", TimeSpan.FromSeconds(5), options, null);

            //    View view = snackbar..getView();
            //    FrameLayout.LayoutParams params = (FrameLayout.LayoutParams)view.getLayoutParams();
            //    params.gravity = Gravity.TOP;
            //    view.setLayoutParams (params);

            //    await snackbar.Show();
            //}




#endif
        }
        catch(Exception ex)
        {
            return;
        }
        return;
    }

    Task CreateNotification(int id, string channel, string Title, string Message, int badege, Dictionary<string, string> payload)
    => this._notificationManager.Send(new Shiny.Notifications.Notification
    {
        Id = id,
        Title = Title,
        Message = Message,
        Channel = channel,
        BadgeCount = badege,
         Payload = payload,
    });

    void CreateChannel(string name, ChannelImportance imp) => this._notificationManager.AddChannel(new Channel
    {
        Identifier = name,
        Description = name,
        Importance = imp,
       // Sound = ChannelSound.High
    });

    public Task OnTokenRefreshed(string token)
 => this.Insert("PUSH TOKEN REFRESH");

    Task Insert(string info)
    {

        return null;
    }



    public string ParseAAction(string y)
    {
        try
        {
            var ai = y.IndexOf("aaction");
            if (ai < 0) return null;
            var fi = y.IndexOf("=", ai + 1);
            var ei = y.IndexOf(";", fi + 1);
            return y.Substring(fi + 1, ei - fi - 1).Trim();
        }
        catch
        {
            return null;
        }
    }

    static string parseAlert(string y)
    {
        try
        {
            var ai = y.IndexOf("title");
            if (ai < 0) return null;
            var fi = y.IndexOf("=", ai + 1);
            var ei = y.IndexOf(";", fi + 1);
            return y.Substring(fi + 1, ei - fi - 1).Trim();
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
            return y.Substring(fi + 1, ei - fi - 1).Trim();
        }
        catch
        {
            return null;
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
            return y.Substring(fi + 1, ei - fi - 1).Trim();
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
            return y.Substring(fi + 1, ei - fi - 1).Trim();
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
            return int.Parse(y.Substring(fi + 1, ei - fi - 1).Trim());
        }
        catch
        {
            return -1;
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
