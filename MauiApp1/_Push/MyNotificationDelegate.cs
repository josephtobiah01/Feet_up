using System;
using System.Threading.Tasks;
using MauiApp1.Areas.Chat.Views;
using Shiny;
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


    public async Task OnEntry(NotificationResponse response)
    {
        try
        {
            bool stop = true;
            //   var m = response.ActionIdentifier;
            var l = response.Notification;

            if (l != null && l.Payload.ContainsKey("aaction"))
            {
                if (l.Payload["aaction"] == "CHAT")
                {
                    await Application.Current.MainPage.Navigation.PushAsync(new ViewHybridChatContentPage());
                }
            }
        }
        catch(Exception ex)
        {

        }
    }
    public async Task OnReceived(NotificationResponse response)
    {
        bool stop = true;
    }

    async Task DoChat(NotificationResponse response)
    {
       
    }
}
