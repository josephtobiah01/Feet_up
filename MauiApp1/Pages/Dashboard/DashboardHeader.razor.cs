using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp1.Areas.BarcodeScanning.Views;
using MauiApp1.Areas.Chat.Views;
using MauiApp1.Areas.Security.Views;
using MauiApp1.Business.DeviceServices;
using MauiApp1.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ParentMiddleWare;
using UserApi.Net7;

namespace MauiApp1.Pages.Dashboard
{
    public partial class DashboardHeader
    {
        [Inject]
        IJSRuntime JSRuntime { get; set; }
        [Parameter]
        public EventCallback<string> Callback { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("renderJqueryComponentsinIndex");
                await JSRuntime.InvokeVoidAsync("SetUnderlineOnInitialize");
            }
        }
        private async Task DashboardButton_Click()
        {
            await Callback.InvokeAsync("DASHBOARD");
        }
        private async Task FeedButton_Click()
        {
            await App.Current.MainPage.Navigation.PopAsync();
        }

    }
}
