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
    private NutrientDashboardContentView _nutrientDashboardContentView;
    private ExerciseStatsContentView _exerciseStatsContentView;
    public DateTime _dateSelected = DateTime.Now;

    private bool _isLoading = false;

    private IDispatcherTimer _dispatcherTimer;

    #endregion

    #region [Methods :: EventHandlers :: Class]

    public RazorHomeContentPage()
    {
        InitializeComponent();
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
        if (_exerciseStatsContentView == null)
        {
            _exerciseStatsContentView = new ExerciseStatsContentView();
        }

        if (_exerciseStatsContentView == null)
        {
            _exerciseStatsContentView.SelectedDateTime = _dateSelected;
        }

        if (_nutrientsIntakeService == null)
        {
            _nutrientsIntakeService = new NutrientsIntakeService();
        }

        if (_nutrientDashboardContentView == null)
        {
            _nutrientDashboardContentView = new NutrientDashboardContentView(_nutrientsIntakeService);
        }

        CheckContentViewLoaded();

        //rootComponent.Parameters =
        //new Dictionary<string, object>
        //{
        //    { "Callback", new EventCallback<string>(null, HandleHeaderButtonClick) }
        //};

        InitializeData();
        InitializeControls();
        InitializeSelectDate();
    }

    private void ContentPage_Unloaded(object sender, EventArgs e)
    {
        UnloadPage();
        DisposeControls();
    }

    private void InitializeData()
    {

    }

    private void InitializeControls()
    {
        RazorHomeViewModel.isFromRazorHomeContentPage = true;
        RazorHomeViewModel.isNavigatedToProfilePage = false;

        _dispatcherTimer = Dispatcher.CreateTimer();
        _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(1000);
        _dispatcherTimer.Tick += Timer_Tick;
        _dispatcherTimer.Start();

        //LoadNutrientsDashboard();
        //LoadExerciseStats();
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
            UpdateExerciseStatsView();
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

    private void LoaderRectangleTapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        return;
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        TimerTick();
    }

    #endregion

    #region [Methods :: Tasks]

    private void HandleDashboardButtonClick()
    {
        LoadNutrientsDashboard();
        LoadExerciseStats();
        //_nutrientsIntakeService = new NutrientsIntakeService();
        //_nutrientDashboardContentView = new NutrientDashboardContentView(_nutrientsIntakeService);
        //_exerciseStatsContentView = new ExerciseStatsContentView();
        //_exerciseStatsContentView.SelectedDateTime = _dateSelected;

        //if (this.NutrientsBarChartLayout != null)
        //{
        //    if (this.NutrientsBarChartLayout.Children.Count <= 0)
        //    {
        //        this.NutrientsBarChartLayout.Add(_nutrientDashboardContentView);
        //    }
        //}

        //if (this.ExerciseBarChartLayout != null)
        //{
        //    if (this.ExerciseBarChartLayout.Children.Count <= 0)
        //    {
        //        this.ExerciseBarChartLayout.Add(_exerciseStatsContentView);
        //    }
        //}

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

    private void LoadNutrientsDashboard()
    {

        if (this.NutrientsBarChartLayout != null)
        {
            if (this.NutrientsBarChartLayout.Children.Count <= 0)
            {
                this.NutrientsBarChartLayout.Add(_nutrientDashboardContentView);
            }
        }

    }

    private void LoadExerciseStats()
    {
        if (this.ExerciseBarChartLayout != null)
        {
            if (this.ExerciseBarChartLayout.Children.Count <= 0)
            {
                this.ExerciseBarChartLayout.Add(_exerciseStatsContentView);
            }
        }
    }

    private void UpdateExerciseStatsView()
    {
        if (_exerciseStatsContentView != null)
        {
            _exerciseStatsContentView.SelectedDateTime = _dateSelected;
            _exerciseStatsContentView.RefreshData();
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

    private void CheckContentViewLoaded()
    {
        //bool blazorWebViewLoaded = true;
        //bool exerciseContentViewLoaded = false;
        //bool nutrientsDashboardContentViewLoaded = false;

        //try
        //{
        //    while (blazorWebViewLoaded == false &&
        //   exerciseContentViewLoaded == false &&
        //   nutrientsDashboardContentViewLoaded == false)
        //    {
        //        blazorWebViewLoaded = this.BlazorWebView.IsLoaded;
        //        exerciseContentViewLoaded = this._exerciseStatsContentView.IsLoaded;
        //        nutrientsDashboardContentViewLoaded = this._nutrientDashboardContentView.IsLoaded;
        //        this.LoaderGrid.IsVisible = true;
        //        this.LoadingActivityIndicator.IsVisible = true;
        //        //await Task.Delay(1);
        //    }

        //    this.LoaderGrid.IsVisible = false;
        //    this.LoadingActivityIndicator.IsVisible = false;
        //}
        //catch (Exception ex)
        //{

        //}


        //bool blazorWebViewLoaded = false;
        //await Task.Run(() =>
        //{
        //    while(blazorWebViewLoaded == false)
        //    {
        //        Dispatcher.Dispatch(() =>
        //        {
        //            blazorWebViewLoaded = this.BlazorWebView.IsLoaded;
        //            this.LoaderGrid.IsVisible = true;
        //            this.LoadingActivityIndicator.IsVisible = true;

        //        });                
        //    }

        //    Dispatcher.Dispatch(() =>
        //    {
        //        this.LoaderGrid.IsVisible = false;
        //        this.LoadingActivityIndicator.IsVisible = false;

        //    });
        //});
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
            UpdateExerciseStatsView();

            RazorHomeViewModel.isFromRazorHomeContentPage = true;
            RazorHomeViewModel.isNavigatedToProfilePage = false;
        }

    }

    private void TimerTick()
    {
        try
        {
            this.LoaderRectangle.IsVisible = true;
            this.LoadingActivityIndicator.IsVisible = true;
            if (this.BlazorWebView != null)
            {
                if(this.BlazorWebView.IsLoaded == true)
                {
                    LoadExerciseStats();
                    if(_exerciseStatsContentView != null)
                    {
                        if(_exerciseStatsContentView._isLoading == false)
                        {
                            LoadNutrientsDashboard();
                            if(_nutrientDashboardContentView != null)
                            {
                                if(_nutrientDashboardContentView.IsLoaded == true)
                                {
                                    this.LoaderRectangle.IsVisible = false;
                                    this.LoadingActivityIndicator.IsVisible = false;
                                    StopTimer();
                                }
                            }                            
                        }
                    }     
                }
            }
        }
        catch (Exception ex)
        {
            this.LoaderRectangle.IsVisible = false;
            this.LoadingActivityIndicator.IsVisible = false;
            if (_dispatcherTimer != null)
            {
                _dispatcherTimer.Stop();
            }

        }
        finally
        {
        }
    }

    private void StopTimer()
    {
        if (_dispatcherTimer != null)
        {
            _dispatcherTimer.Stop();
        }
    }

    private void DisposeControls()
    {
        if (_dispatcherTimer != null)
        {
            _dispatcherTimer.Stop();
            _dispatcherTimer = null;
        }
    }

    #endregion
    private async void Feed_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

   
}