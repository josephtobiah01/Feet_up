using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using MauiApp1.Areas.Nutrient.Views;
using ExerciseApi.Net7;
using ParentMiddleWare.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using FeedApi.Net7.Models;
using ImageApi.Net7;
using MauiApp1._Push;

namespace MauiApp1.Pages.Popups
{

    public partial class SkipPopup
    {
        [Parameter]
        public string DisplayPopup { get; set; } = "none";

        public async Task ConfirmSkip()
        {
            if (PushNavigationHelper.RootPage! != null)
            {
                await PushNavigationHelper.RootPage.SkipCurrentFeedItem();
            }
            await ClosePopup();
        }
        public async Task ClosePopup()
        {
            if (PushNavigationHelper.RootPage! != null)
            {
                await PushNavigationHelper.RootPage.CloseSkipPopup();
            }
            DisplayPopup = "none";
        }
    }
}