using DeviceIntegration.Client.Business;
using DeviceIntegration.Common.Data;
using MauiApp1.Areas.Dashboard.Resources.Drawables;

namespace MauiApp1.Areas.Dashboard.Views;

public partial class ExerciseDashboardContentView : ContentView
{
    #region [Fields]



    #endregion

    #region [Methods :: EventHandlers :: Class]

    public ExerciseDashboardContentView()
    {
        InitializeComponent();
    }

    private void ContentView_Loaded(object sender, EventArgs e)
    {
        InitializeData();
        InitializeControl();
    }

    private void InitializeData()
    {
        LoadStepsCountPerDaysOfWeekBarChart();
        LoadStepsCountPerDayBarChart();
        LoadCaloriesExpendedPerDaysOfWeekBarChart();
        LoadCaloriesExpendedPerDayBarChart();
    }

    private void InitializeControl()
    {

    }

    #endregion

    #region [Methods :: EventHandlers :: Controls]



    #endregion

    #region [Methods :: Tasks]

    private void LoadStepsCountPerDaysOfWeekBarChart()
    {
        DateTimeOffset startDateTimeOffset = new DateTimeOffset(2023, 06, 12, 0, 0, 0, TimeSpan.FromHours(8));
        DateTimeOffset endDateTimeOffset = new DateTimeOffset(2023, 06, 18, 23, 59, 59, TimeSpan.FromHours(8));

        GoogleFitStepsCountDataManager googleFitStepsCountDataManager = new GoogleFitStepsCountDataManager();
        List<StepsCountDataPointViewItem> stepsCountDataPointViewItems = googleFitStepsCountDataManager.GetLatestDailyStepsCountDataByDateRange(startDateTimeOffset, endDateTimeOffset);

        StepsCountPerDaysOfWeekBarChartDrawable stepsCountPerDaysOfWeekBarChartDrawable = new StepsCountPerDaysOfWeekBarChartDrawable();
        stepsCountPerDaysOfWeekBarChartDrawable.StepsCountDataPointViewItems = stepsCountDataPointViewItems;
        stepsCountPerDaysOfWeekBarChartDrawable.StartDateTimeOffset = startDateTimeOffset;
        //stepsCountPerDaysOfWeekBarChartDrawable.EndDateTimeOffset = endDateTimeOffset;

        this.StepsCountPerDaysOfWeekGraphicsView.Drawable = stepsCountPerDaysOfWeekBarChartDrawable;

        //Application.Current.MainPage.Dispatcher.Dispatch(() =>
        //{
        //    this.StepsCountPerDaysOfWeekGraphicsView.Invalidate();
        //});
    }

    private void LoadStepsCountPerDayBarChart()
    {
        DateTimeOffset startDateTimeOffset = new DateTimeOffset(2023, 06, 12, 0, 0, 0, TimeSpan.FromHours(8));
        DateTimeOffset endDateTimeOffset = new DateTimeOffset(2023, 06, 12, 23, 59, 59, TimeSpan.FromHours(8));

        GoogleFitStepsCountDataManager googleFitStepsCountDataManager = new GoogleFitStepsCountDataManager();
        List<StepsCountDataPointViewItem> stepsCountDataPointViewItems = googleFitStepsCountDataManager.GetLatestDailyStepsCountDataByDateRange(startDateTimeOffset, endDateTimeOffset);


        StepsCountPerDayBarChartDrawable stepsCountPerDayBarChartDrawable = new StepsCountPerDayBarChartDrawable();
        stepsCountPerDayBarChartDrawable.StepsCountDataPointViewItem = stepsCountDataPointViewItems.FirstOrDefault();
        stepsCountPerDayBarChartDrawable.StartDateTimeOffset = startDateTimeOffset;
        //stepsCountPerDayBarChartDrawable.EndDateTimeOffset = endDateTimeOffset;

        this.StepsCountPerDayGraphicsView.Drawable = stepsCountPerDayBarChartDrawable;

        //Application.Current.MainPage.Dispatcher.Dispatch(() =>
        //{
        //    this.StepsCountPerDayGraphicsView.Invalidate();
        //});
    }

    private void LoadCaloriesExpendedPerDaysOfWeekBarChart()
    {
        DateTimeOffset startDateTimeOffset = new DateTimeOffset(2023, 06, 12, 0, 0, 0, TimeSpan.FromHours(8));
        DateTimeOffset endDateTimeOffset = new DateTimeOffset(2023, 06, 18, 23, 59, 59, TimeSpan.FromHours(8));

        GoogleFitCaloriesExpendedDataManager googleFitCaloriesExpendedDataManager = new GoogleFitCaloriesExpendedDataManager();
        List<CaloriesExpendedDataPointViewItem> caloriesExpendedDataPointViewItems = googleFitCaloriesExpendedDataManager.GetLatestDailyCaloriesExpendedDataByDateRange(startDateTimeOffset, endDateTimeOffset);


        CaloriesExpendedPerDaysOfWeekStackedBarChartDrawable caloriesExpendedPerDaysOfWeekStackedBarChartDrawable = new CaloriesExpendedPerDaysOfWeekStackedBarChartDrawable();
        caloriesExpendedPerDaysOfWeekStackedBarChartDrawable.CaloriesExpendedDataPointViewItems = caloriesExpendedDataPointViewItems;
        caloriesExpendedPerDaysOfWeekStackedBarChartDrawable.StartDateTimeOffset = startDateTimeOffset;
        //caloriesExpendedPerDaysOfWeekStackedBarChartDrawable.EndDateTimeOffset = endDateTimeOffset;

        this.CaloriesBurnedPerDaysOfWeekGraphicsView.Drawable = caloriesExpendedPerDaysOfWeekStackedBarChartDrawable;

        //Application.Current.MainPage.Dispatcher.Dispatch(() =>
        //{
        //    this.CaloriesBurnedPerDaysOfWeekGraphicsView.Invalidate();
        //});
    }

    private void LoadCaloriesExpendedPerDayBarChart()
    {
        DateTimeOffset startDateTimeOffset = new DateTimeOffset(2023, 06, 12, 0, 0, 0, TimeSpan.FromHours(8));
        DateTimeOffset endDateTimeOffset = new DateTimeOffset(2023, 06, 12, 23, 59, 59, TimeSpan.FromHours(8));

        GoogleFitCaloriesExpendedDataManager googleFitCaloriesExpendedDataManager = new GoogleFitCaloriesExpendedDataManager();
        List<CaloriesExpendedDataPointViewItem> caloriesExpendedDataPointViewItems = googleFitCaloriesExpendedDataManager.GetLatestDailyCaloriesExpendedDataByDateRange(startDateTimeOffset, endDateTimeOffset);


        CaloriesExpendedPerDayStackedBarChartDrawable caloriesExpendedPerDayStackedBarChartDrawable = new CaloriesExpendedPerDayStackedBarChartDrawable();
        caloriesExpendedPerDayStackedBarChartDrawable.CaloriesExpendedDataPointViewItems = caloriesExpendedDataPointViewItems;
        caloriesExpendedPerDayStackedBarChartDrawable.StartDateTimeOffset = startDateTimeOffset;
        //caloriesExpendedPerDaysOfWeekStackedBarChartDrawable.EndDateTimeOffset = endDateTimeOffset;

        this.CaloriesExpendedPerDayGraphicsView.Drawable = caloriesExpendedPerDayStackedBarChartDrawable;

        //Application.Current.MainPage.Dispatcher.Dispatch(() =>
        //{
        //    this.CaloriesExpendedPerDayGraphicsView.Invalidate();
        //});
    }

    #endregion
}