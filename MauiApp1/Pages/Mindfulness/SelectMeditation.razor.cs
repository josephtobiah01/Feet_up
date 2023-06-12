using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using MauiApp1.Areas.Exercise.Views;
using ExerciseApi.Net7;
using ParentMiddleWare.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MauiApp1.Pages.Mindfulness
{

    public partial class SelectMeditation 
    {


        

        public async void ClosePage()
        {
            await App.Current.MainPage.Navigation.PopAsync();
        }

    }
}