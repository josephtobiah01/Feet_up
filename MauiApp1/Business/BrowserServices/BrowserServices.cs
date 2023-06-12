
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace MauiApp1.Business.BrowserServices
{
    public class BrowserServices
    {

        private readonly IJSRuntime _javascriptRuntime;

        public BrowserServices(IJSRuntime javascriptRuntime)
        {
            _javascriptRuntime = javascriptRuntime;
        }

        public async Task<BrowserDimension> GetDimensions()
        {
            return await _javascriptRuntime.InvokeAsync<BrowserDimension>("GetBrowserDimensions");
        }

    }
}
