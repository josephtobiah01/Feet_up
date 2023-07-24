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

    public partial class SnoozePopup 
    {
        [Parameter]
        public string DisplayPopup { get; set; } = "none";
        [Parameter]
        public bool OpenMenuOnClose { get; set; } = false;

        public async Task thirty_onclick()
        {
            await PushNavigationHelper.RootPage.SnoozeByAmount(30);
            await ClosePopup();
        }

        public async Task fourtyfive_onclick()
        {
            await PushNavigationHelper.RootPage.SnoozeByAmount(45);
            await ClosePopup();
        }

        public async Task sixty_onclick()
        {
            await PushNavigationHelper.RootPage.SnoozeByAmount(60);
            await ClosePopup();
        }

        public async Task onehundredtwenty_onclick()
        {
            await PushNavigationHelper.RootPage.SnoozeByAmount(120);
            await ClosePopup();
        }

        public async Task ClosePopup()
        {
            if (PushNavigationHelper.RootPage != null)
            {
                if (OpenMenuOnClose)
                {
                    await PushNavigationHelper.RootPage.OpenMenuPopup();
                }
                await PushNavigationHelper.RootPage.CloseSnoozePopup();
            }
            DisplayPopup = "none";
        }
    }
}