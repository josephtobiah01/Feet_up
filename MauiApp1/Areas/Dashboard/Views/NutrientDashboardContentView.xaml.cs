using MauiApp1.Areas.Dashboard.DataManager;
using MauiApp1.Areas.Dashboard.Resources.Drawables;
using MauiApp1.Areas.Dashboard.ViewModel;
using MauiApp1.Models;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.Areas.Chat.Views;
using MauiApp1.Interfaces;
using MauiApp1.Services;
using System.Windows.Input;

namespace MauiApp1.Areas.Dashboard.Views;

public partial class NutrientDashboardContentView : ContentView, INotifyPropertyChanged
{

    #region [Fields]

    
    
    private DateTime _selectedDate = DateTime.Today;
    private int _targetCalories;
    private double _currentCalories;
    private INutrientsIntakeService _nutrientsIntakeService;
    private ICommand _navigateTo2ndPage;
  //  private NutrientsDailyIntakeContentPage _nutrientsDailyIntakeContentPage;


    #endregion

    #region Properties

    public event PropertyChangedEventHandler PropertyChanged;

    public ICommand NavigateTo2ndPage
    {
        get { return _navigateTo2ndPage; }
        set
        {
            if (_navigateTo2ndPage != value)
            {
                _navigateTo2ndPage = value;
                OnPropertyChanged(nameof(NavigateTo2ndPage));
            }
        }
    }


    public int TargetCalories
    {
        get => _targetCalories;
        set
        {
            if (_targetCalories != value)
            {
                _targetCalories = value;
                OnPropertyChanged(nameof(TargetCalories));
            }
        }
    }

    public double CurrentCalories
    {
        get => _currentCalories;
        set
        {
            if (_currentCalories != value)
            {
                _currentCalories = value;
                OnPropertyChanged(nameof(CurrentCalories));

            }
        }
    }


    public DateTime SelectedDate
    {
        get => _selectedDate;
        set
        {
            if (_selectedDate != value)
            {
                _selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));
            }
        }
    }


    #endregion Properties

    #region [Methods :: EventHandlers :: Class]

    #region Contstructor

    public NutrientDashboardContentView(INutrientsIntakeService nutrientsIntakeService)
    {
        InitializeComponent();
        BindingContext = this;

        NavigateTo2ndPage = new Command(NavigateTo2ndPageCommand);
        _nutrientsIntakeService = nutrientsIntakeService;

        CurrentCalories = 0;
        TargetCalories = 0;
    }

    

    #endregion Constructor



    private void InitializeData()
    {

       // _nutrientsIntakeService = new NutrientsIntakeService();
     //   _nutrientsDailyIntakeContentPage = new NutrientsDailyIntakeContentPage(_nutrientsIntakeService);

        
        //OnPropertyChanged(nameof(SelectedDate));
        
    }


    

    #endregion

    #region [Methods :: EventHandlers :: Controls]



    #endregion

    #region [Methods :: Tasks]

    
    protected override void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }



    private async Task ShowAlertMessage(string title, string message)
    {

        await Application.Current.MainPage.DisplayAlert(title, message, "OK").ConfigureAwait(false);

    }



    private async void NavigateTo2ndPageCommand()
    {
        NutrientsDailyIntakeContentPage _nutrientsDailyIntakeContentPage = new NutrientsDailyIntakeContentPage(_nutrientsIntakeService);
        _nutrientsDailyIntakeContentPage.SelectedDate = SelectedDate;
        await _nutrientsDailyIntakeContentPage.LoadTotalNutrientsBarChart();
        await Application.Current.MainPage.Navigation.PushAsync(_nutrientsDailyIntakeContentPage, true);

        
    }



    public async Task<List<NutrientsIntakeViewItem>> LoadTotalNutrientsBarChart()
    {
        try
        {
            List<NutrientsIntakeViewItem> nutrientsIntakeViewItems = new List<NutrientsIntakeViewItem>();

            nutrientsIntakeViewItems = await _nutrientsIntakeService.LoadTotalNutrientsBarChart(SelectedDate);

            TotalNutrientsBarChartDrawable totalNutrientsBarChartDrawable = new TotalNutrientsBarChartDrawable();

            totalNutrientsBarChartDrawable.ShowTargetLine = true;

            foreach (var item in nutrientsIntakeViewItems)
            {
                totalNutrientsBarChartDrawable.ProteinToDisplay = item.ProteinToDisplay;
                totalNutrientsBarChartDrawable.CarbohydratesToDisplay = item.CarbohydratesToDisplay;
                totalNutrientsBarChartDrawable.FatToDisplay = item.FatToDisplay;
                totalNutrientsBarChartDrawable.TargetCalories = item.TargetCalories; 

                CurrentCalories = item.TranscribedCalories; //.ProteinToDisplay + item.CarbohydratesToDisplay + item.FatToDisplay;
                TargetCalories = item.TargetCalories;
            }

            this.TotalNutrientsGraphicsView.Drawable = totalNutrientsBarChartDrawable;

            Application.Current.MainPage.Dispatcher.Dispatch(() =>
            {
                this.TotalNutrientsGraphicsView.Invalidate();
            });

            return nutrientsIntakeViewItems;
        }
        catch(Exception ex)
        {
            return new List<NutrientsIntakeViewItem>();
        }
    }
    #endregion


}