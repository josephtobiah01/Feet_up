using Shiny.Push;

namespace Sample;


public class MyPushDelegate : IPushDelegate
{
    readonly SampleSqliteConnection conn;
    readonly IPushManager pushManager;


    public MyPushDelegate(SampleSqliteConnection conn, IPushManager pushManager)
    {
        this.conn = conn;
        this.pushManager = pushManager;
    }

    //public async Task OnEntry(PushEntryArgs args)
    //{
    //    // fires when the user taps on a push notification
    //}

    public async Task OnReceived(IDictionary<string, string> data)
    {
        var x = false;
    }

    public async Task OnTokenChanged(string token)
    {
        // fires when a push notification change is set by the operating system or provider
    }

    public Task OnEntry(PushNotification push)
        => this.Insert("PUSH ENTRY");

    public Task OnReceived(PushNotification push)
        => this.Insert("PUSH RECEIVED");

    public Task OnTokenRefreshed(string token)
        => this.Insert("PUSH TOKEN REFRESH");

    Task Insert(string info) => this.conn.InsertAsync(new ShinyEvent
    {
        Text = info,
        Detail = "Token: " + this.pushManager.RegistrationToken,
        Timestamp = DateTime.UtcNow
    });
}
