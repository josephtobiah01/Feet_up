using MauiApp1.Areas.Biodata.Views;
using MauiApp1.Areas.Dashboard.ViewModel;
using MauiApp1.Areas.Dashboard.Views;
using MauiApp1.Areas.Profile.Views;
using MauiApp1.Business.DeviceServices;
using MauiApp1.Interfaces;
using MauiApp1.Services;
using Microsoft.AspNetCore.Components;

namespace MauiApp1.Areas.Overview.Views;

public partial class RazorHomeContentPage
{
    #region [Fields]

    INutrientsIntakeService _nutrientsIntakeService;
    NutrientDashboardContentView _nutrientDashboardContentView;
    ExerciseStatsContentView _exerciseStatsContentView;
    public DateTime _dateSelected = DateTime.Now;

    private bool _isLoading = false;

    #endregion

    #region [Methods :: EventHandlers :: Class]

    public RazorHomeContentPage()
    {
        InitializeComponent();

        HandleDashboardButtonClick();


        rootComponent.Parameters =
        new Dictionary<string, object>
        {
            { "Callback", new EventCallback<string>(null, HandleHeaderButtonClick) }
        };

        InitializeSelectDate();
    }
    public void HandleHeaderButtonClick(string input)
    {
        if (input == "DASHBOARD")
        {
            HandleDashboardButtonClick();
        }
        else if (input == "BIODATA")
        {
            HandleBiodataButtonClick();
        }
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        ReloadPage(args);
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        InitializeData();
        CheckBlazorLoaded();
    }

    private void ContentPage_Unloaded(object sender, EventArgs e)
    {
        UnloadPage();
    }

    private void InitializeData()
    {
        RazorHomeViewModel.isFromRazorHomeContentPage = true;
        RazorHomeViewModel.isNavigatedToProfilePage = false;
    }

    #region Datepicker

    private async void InitializeSelectDate()
    {
        await SetDate(_dateSelected);
    }
    public async Task SetDate(DateTime? DateInput)
    {
        string formattedDate = string.Empty;
        string numberSuffix = string.Empty;
        string monthShort = string.Empty;
        if (DateInput != null)
        {
            //System.Diagnostics.Debug.WriteLine(DateInput.ToString());

            _dateSelected = DateInput.GetValueOrDefault();
            monthShort = _dateSelected.ToString("MMM");
            numberSuffix = GetDayNumberSuffix(_dateSelected);
            formattedDate = string.Format("{0} {1}, {2}", _dateSelected.Day + numberSuffix, monthShort, _dateSelected.DayOfWeek);
            this.DatePickerButton.Text = formattedDate;

            //do other stuff with _dateSelected as the selected Date
            //@joseph, do ur stuff here.
            await UpdateExerciseStatsView();
            await UpdateNutrientsBarChart();
        }
    }
    private string GetDayNumberSuffix(DateTime date)
    {
        int day = date.Day;

        switch (day)
        {
            case 1:
            case 21:
            case 31:
                return "st";

            case 2:
            case 22:
                return "nd";

            case 3:
            case 23:
                return "rd";

            default:
                return "th";
        }
    }
    #endregion

    private void Initialize()
    {
        _nutrientsIntakeService = new NutrientsIntakeService();
        _nutrientDashboardContentView = new NutrientDashboardContentView(_nutrientsIntakeService);
        _exerciseStatsContentView = new ExerciseStatsContentView();
    }

    #endregion

    #region [Methods :: EventHandlers :: Controls]

    private void DatePickerButton_Clicked(object sender, EventArgs e)
    {
        this.CalendarDXPopup.IsOpen = true;
    }
    private async void DatePickerLeft_Clicked(object sender, EventArgs e)
    {
        _dateSelected = _dateSelected.AddDays(-1);
        this.Calendar.SelectedDate = _dateSelected;
        await SetDate(_dateSelected);
    }
    private async void DatePickerRight_Clicked(object sender, EventArgs e)
    {
        _dateSelected = _dateSelected.AddDays(1);
        this.Calendar.SelectedDate = _dateSelected;
        await SetDate(_dateSelected);
    }
    public async void DateSelected(object sender, EventArgs e)
    {
        this.CalendarDXPopup.IsOpen = false;
        await SetDate(this.Calendar.SelectedDate);
    }
    #endregion

    #region [Methods :: Tasks]

    private void HandleDashboardButtonClick()
    {
        _nutrientsIntakeService = new NutrientsIntakeService();
        _nutrientDashboardContentView = new NutrientDashboardContentView(_nutrientsIntakeService);
        _exerciseStatsContentView = new ExerciseStatsContentView();
        _exerciseStatsContentView.SelectedDateTime = _dateSelected;

        if (this.NutrientsBarChartLayout != null)
        {
            if (this.NutrientsBarChartLayout.Children.Count <= 0)
            {
                this.NutrientsBarChartLayout.Add(_nutrientDashboardContentView);
            }
        }

        if (this.ExerciseBarChartLayout != null)
        {
            if (this.ExerciseBarChartLayout.Children.Count <= 0)
            {
                this.ExerciseBarChartLayout.Add(_exerciseStatsContentView);
            }
        }

        //if (this.BiodataContentStackLayout != null)
        //{
        //    this.BiodataContentStackLayout.IsVisible = false;
        //}
    }

    private void HandleBiodataButtonClick()
    {
        if (this.BiodataContentStackLayout != null)
        {
            if (this.BiodataContentStackLayout.Children.Count <= 0)
            {
                this.BiodataContentStackLayout.Add(new OverviewBiodata());
            }
            this.BiodataContentStackLayout.IsVisible = true;
        }
        //if (this.DashboardContentStackLayout != null)
        //{
        //    this.DashboardContentStackLayout.IsVisible = false;
        //}
    }

    private async Task UpdateExerciseStatsView()
    {
        if(_exerciseStatsContentView != null)
        {
            _exerciseStatsContentView.SelectedDateTime = _dateSelected;
           await _exerciseStatsContentView.RefreshData();
        }
    }

    private async Task UpdateNutrientsBarChart()
    {
        if (_nutrientDashboardContentView != null)
        {
            _nutrientDashboardContentView.SelectedDate = _dateSelected;
            await _nutrientDashboardContentView.LoadTotalNutrientsBarChart();
        }
    }

    private async void CheckBlazorLoaded()
    {
        bool blazorWebViewLoaded = false;
        await Task.Run(() =>
        {
            while(blazorWebViewLoaded == false)
            {
                Dispatcher.Dispatch(() =>
                {
                    blazorWebViewLoaded = this.BlazorWebView.IsLoaded;
                    this.LoaderGrid.IsVisible = true;
                    this.LoadingActivityIndicator.IsVisible = true;

                });                
            }

            Dispatcher.Dispatch(() =>
            {
                this.LoaderGrid.IsVisible = false;
                this.LoadingActivityIndicator.IsVisible = false;

            });
        });
    }

    private void UnloadPage()
    {
        RazorHomeViewModel.isFromRazorHomeContentPage = false;
        RazorHomeViewModel.isNavigatedToProfilePage = false;
    }

    private async void ReloadPage(NavigatedToEventArgs navigatedToEventArgs)
    {        

        if (RazorHomeViewModel.isFromRazorHomeContentPage == true &&
            RazorHomeViewModel.isNavigatedToProfilePage == true)
        {
            await UpdateNutrientsBarChart();
            await UpdateExerciseStatsView();

            RazorHomeViewModel.isFromRazorHomeContentPage = true;
            RazorHomeViewModel.isNavigatedToProfilePage = false;
        }
        
    }

    #endregion


}