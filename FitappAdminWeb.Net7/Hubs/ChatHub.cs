using Microsoft.AspNetCore.SignalR;

namespace FitappAdminWeb.Net7.Hubs
{
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            //tentative: 
            //SignalR solution (native hub)
            //retrieve user messages here, then setup update worker that polls updates from db then broadcast it to caller.
            //Query must be as optimized as possible. This will need some kind of dictionary to hold the workers somewhere.
            //BE VERY CAREFUL WITH THIS! On Disconnection, the workers MUST be deactivated or else it will cause unnecessary DB calls
            
            await base.OnConnectedAsync();
        }

        public async Task Send(string name, string message)
        {
            await Clients.All.SendAsync("broadcastMessage", name, message);
        }
    }
}
