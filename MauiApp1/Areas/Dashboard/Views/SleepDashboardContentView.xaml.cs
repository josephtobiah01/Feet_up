using DeviceIntegration.Client.Business;
using DeviceIntegration.Common.Data;
using MauiApp1.Areas.Dashboard.Resources.Drawables;

namespace MauiApp1.Areas.Dashboard.Views;

public partial class SleepDashboardContentView : ContentView
{
    #region [Fields]



    #endregion

    #region [Methods :: EventHandlers :: Class]

    public SleepDashboardContentView()
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

        LoadSleepSegmentPerDaysOfWeekBarChart();
        //LoadSleepSegmentPerDayBarChart();
    }

    private void InitializeControl()
    {

    }

    #endregion

    #region [Methods :: EventHandlers :: Controls]



    #endregion

    #region [Methods :: Tasks]

    private void LoadSleepSegmentPerDaysOfWeekBarChart()
    {
        DateTimeOffset startDateTimeOffset = new DateTimeOffset(2023, 06, 12, 0, 0, 0, TimeSpan.FromHours(8));
        DateTimeOffset endDateTimeOffset = new DateTimeOffset(2023, 06, 18, 23, 59, 59, TimeSpan.FromHours(8));

        GoogleFitSleepSegmentDataManager googleFitSleepSegmentDataManager = new GoogleFitSleepSegmentDataManager();
        List<SleepSegmentDataPointViewItem> sleepSegmentDataPointViewItems = googleFitSleepSegmentDataManager.GetLatestDailySleepSegmentDataByDateRange(startDateTimeOffset, endDateTimeOffset);


        SleepSegmentPerDaysOfWeekStackedBarChartDrawable sleepSegmentPerDaysOfWeekStackedBarChartDrawable = new SleepSegmentPerDaysOfWeekStackedBarChartDrawable();
        sleepSegmentPerDaysOfWeekStackedBarChartDrawable.SleepSegmentDataPointViewItems = sleepSegmentDataPointViewItems;
        sleepSegmentPerDaysOfWeekStackedBarChartDrawable.StartDateTimeOffset = startDateTimeOffset;
        //sleepSegmentPerDaysOfWeekStackedBarChartDrawable.EndDateTimeOffset = endDateTimeOffset;

        this.SleepSegmentPerDaysOfWeekGraphicsView.Drawable = sleepSegmentPerDaysOfWeekStackedBarChartDrawable;

        //Application.Current.MainPage.Dispatcher.Dispatch(() =>
        //{
        //    this.SleepSegmentPerDaysOfWeekGraphicsView.Invalidate();
        //});
    }

    private void LoadSleepSegmentPerDayBarChart()
    {
        DateTimeOffset startDateTimeOffset = new DateTimeOffset(2023, 06, 15, 0, 0, 0, TimeSpan.FromHours(8));
        DateTimeOffset endDateTimeOffset = new DateTimeOffset(2023, 06, 15, 23, 59, 59, TimeSpan.FromHours(8));

        GoogleFitSleepSegmentDataManager googleFitSleepSegmentDataManager = new GoogleFitSleepSegmentDataManager();
        List<SleepSegmentDataPointViewItem> sleepSegmentDataPointViewItems = googleFitSleepSegmentDataManager.GetLatestDailySleepSegmentDataByDateRange(startDateTimeOffset, endDateTimeOffset);


        SleepSegmentPerDayStackedBarChartDrawable sleepSegmentPerDayStackedBarChartDrawable = new SleepSegmentPerDayStackedBarChartDrawable();
        sleepSegmentPerDayStackedBarChartDrawable.SleepSegmentDataPointViewItems = sleepSegmentDataPointViewItems;
        sleepSegmentPerDayStackedBarChartDrawable.StartDateTimeOffset = startDateTimeOffset;
        //sleepSegmentPerDaysOfWeekStackedBarChartDrawable.EndDateTimeOffset = endDateTimeOffset;

        this.SleepSegmentPerDayGraphicsView.Drawable = sleepSegmentPerDayStackedBarChartDrawable;

        //Application.Current.MainPage.Dispatcher.Dispatch(() =>
        //{
        //    this.SleepSegmentPerDayGraphicsView.Invalidate();
        //});
    }

    #endregion
}