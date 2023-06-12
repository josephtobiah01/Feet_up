using Shiny.Hosting;
using Shiny.Push;

public class PushRegistration
{
    public static async Task<string> CheckPermission()
    {
        try
        {
            //var push = Host.Current.Services.GetService<IPushManager>();

            //var result = await push.RequestAccess();
            //if (result.Status == AccessState.Available)
            //{

            //    // good to go
            //    // var token1 = service.NativeToken;
            //    //  var token2 = service.InstallationId;
            //    // you should send this to your server with a userId attached if you want to do custom work
            //    return result.RegistrationToken;
            //}
            return null;
        }
        catch(Exception e)
        {
            return null;
        }

    }
}