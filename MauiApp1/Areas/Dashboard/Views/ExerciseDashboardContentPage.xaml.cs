
using MauiApp1.Business;
using ParentMiddleWare;

namespace MauiApp1.Areas.Dashboard.Views;

public partial class ExerciseDashboardContentPage : ContentPage
{

    #region [Fields]

    private ExerciseDashboardContentView _exerciseDashboardContentView;
    private ExerciseFeedListContentView _exerciseFeedListContentView;

    #endregion

    #region [Methods :: EventHandlers :: Class]

    public ExerciseDashboardContentPage()
    {
        InitializeComponent();
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        InitializeData();
        IntializeControl();
    }

    #endregion

    #region [Methods :: EventHandlers :: Controls]

    private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {

    }

    private void BackButton_Clicked(object sender, EventArgs e)
    {
        GoBack();
    }

    private void InitializeData()
    {
        _exerciseFeedListContentView = new ExerciseFeedListContentView();
        _exerciseFeedListContentView.SelectedDateTime = this.SelectedDateTime;
        _exerciseDashboardContentView = new ExerciseDashboardContentView();
        _exerciseDashboardContentView.SelectedDateTime = this.SelectedDateTime;
    }

    private void IntializeControl()
    {
        LoadExerciseFeedItem();
        InitializeSelectedDate();
        //SetSelectedDateLabel();
    }

    #endregion

    #region [Methods :: Tasks]

    private void LoadExerciseFeedItem()
    {

        if (this.ChartVerticalStackLayout != null)
        {
            if (this.ChartVerticalStackLayout.Children.Contains(_exerciseFeedListContentView) == false)
            {
                this.ChartVerticalStackLayout.Insert(0, _exerciseFeedListContentView);
            }

            if (this.ChartVerticalStackLayout.Children.Contains(_exerciseDashboardContentView) == false)
            {
                this.ChartVerticalStackLayout.Insert(1, _exerciseDashboardContentView);
            }
            else
            {
                
            }      
            this.ChartVerticalStackLayout.IsVisible = true;
        }
    }

    private void RefreshExerciseFeedItemsContentView()
    {
        if(_exerciseFeedListContentView != null)
        {
            _exerciseFeedListContentView.SelectedDateTime = this.SelectedDateTime;
            _exerciseFeedListContentView.RefreshData();
        }
    }

    private void RefreshExerciseDashboardContentView()
    {
        if (_exerciseDashboardContentView != null)
        {
            _exerciseDashboardContentView.SelectedDateTime = this.SelectedDateTime;
            _exerciseDashboardContentView.RefreshData();
        }
    }
    /*
    private void SetSelectedDateLabel()
    {
        if(this.selectedDateTime.Date == DateTime.Now)
        {
            this.SelectedDatelabel.Text = "Today >";
        }
        else
        {
            this.SelectedDatelabel.Text = this.selectedDateTime.Date.ToString("dddd, MMMM dd, yyyy") +" >";
        }
        
    }
    */

    #region DatePickerStuff
    private async void InitializeSelectedDate()
    {
        await SetDate(this.SelectedDateTime);
    }
    public async Task SetDate(DateTime? DateInput)
    {
        string formattedDate = string.Empty;
        string numberSuffix = string.Empty;
        string monthShort = string.Empty;
        if (DateInput != null)
        {
            //System.Diagnostics.Debug.WriteLine(DateInput.ToString());

            this.SelectedDateTime = DateInput.GetValueOrDefault();
            monthShort = this.SelectedDateTime.ToString("MMM");
            numberSuffix = GetDayNumberSuffix(this.SelectedDateTime);
            formattedDate = string.Format("{0} {1}, {2}", this.SelectedDateTime.Day + numberSuffix, monthShort, this.SelectedDateTime.DayOfWeek);

            this.DatePickerButton.Text = formattedDate;
            //do other stuff with _dateSelected as the selected Date
            //@joseph, do ur stuff here.

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
    private void DatePickerButton_Clicked(object sender, EventArgs e)
    {
        this.CalendarDXPopup.IsOpen = true;
    }
    private async void DatePickerLeft_Clicked(object sender, EventArgs e)
    {
        this.SelectedDateTime = this.SelectedDateTime.AddDays(-1);
        this.Calendar.SelectedDate = this.SelectedDateTime;
        await SetDate(this.SelectedDateTime);

    }
    private async void DatePickerRight_Clicked(object sender, EventArgs e)
    {
        this.SelectedDateTime = this.SelectedDateTime.AddDays(1);
        this.Calendar.SelectedDate = this.SelectedDateTime;
        await SetDate(this.SelectedDateTime);

    }
    public async void DateSelected(object sender, EventArgs e)
    {
        this.CalendarDXPopup.IsOpen = false;
        await SetDate(this.Calendar.SelectedDate);

        RefreshExerciseFeedItemsContentView();
        RefreshExerciseDashboardContentView();
    }
    #endregion



    private async void GoBack()
    {
        await Navigation.PopAsync();

    }    

    #endregion

    #region [Methods :: Properties]

    public DateTime SelectedDateTime;

    #endregion

}