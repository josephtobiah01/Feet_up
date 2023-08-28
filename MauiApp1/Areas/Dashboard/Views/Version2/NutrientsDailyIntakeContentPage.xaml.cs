using DevExpress.Xpo.DB;
using ImageApi.Net7;
using MauiApp1.Areas.Dashboard.Resources.Drawables;
using MauiApp1.Areas.Dashboard.ViewModel;
using MauiApp1.Interfaces;
using MauiApp1.Services;
using ParentMiddleWare.NutrientModels;
using System.Collections.ObjectModel;

namespace MauiApp1.Areas.Dashboard.Views.Version2;

public partial class NutrientsDailyIntakeContentPage : ContentPage
{

    #region Fields

    private INutrientsIntakeService _nutrientsIntakeService;

    private DailyNutrientDetailViewModel _dailyNutrientDetailViewModel;

    public bool _loadedAlready = false;
    public bool _dataLoading = false;

    private MealContentView _mealContentView;

    #endregion

    #region [Methods :: EventHandlers :: Class]

    public NutrientsDailyIntakeContentPage()
    {
        InitializeComponent();
    }

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        if (_loadedAlready)
        {
            return;
        }
        else
        {
            _loadedAlready = true;
        }

        _nutrientsIntakeService = new NutrientsIntakeService();
        _mealContentView = new MealContentView();

        await IntializeData();
        IntializeControl();
        
    }

    private async Task IntializeData()
    {
        try
        {
            if(_dataLoading == true)
            {
                return;
            }

            _dataLoading = true;
            this.LoaderGrid.IsVisible = true;
            this.LoadingActivityIndicator.IsVisible = true;

            DailyNutrientDetails dailyNutrientDetails = await GetDailyNutrientDetails(this.RequestedDate);
            
            GetDailyNutrientDetailViewModel(dailyNutrientDetails);
            await LoadTotalNutrientsBarChart();

            this.BindingContext = _dailyNutrientDetailViewModel;            
        }
        catch (Exception ex)
        {
            //await App.Current.MainPage.DisplayAlert("Retrieve Daily Nutrient Detail", "An error occurred while retrieving daily nutrient detail", "OK");
        }
        finally
        {
            this.LoaderGrid.IsVisible = false;
            this.LoadingActivityIndicator.IsVisible = false;
            _dataLoading = false;
        }

    }

    private void IntializeControl()
    {
        SetDate(_dailyNutrientDetailViewModel.SelectedDate);
        IntializeMealTabClick();


    }

    private void IntializeMealTabClick()
    {
        bool hasSetClicked = false;

        try
        {
            foreach (MealDetailViewModel mealDetail in _dailyNutrientDetailViewModel.MealDetails)
            {
                switch (mealDetail.IsClickable)
                {
                    case true:

                        if(hasSetClicked == false)
                        {
                            mealDetail.IsClicked = true;
                            hasSetClicked = true;
                            LoadPerMealNutrientsBarChart(mealDetail);
                            LoadMealSpecificOverview(mealDetail.MealSpecificNutrientOverView);
                        }
                        break;
                    
                    case false:


                        break;

                }
            }

            if(_dailyNutrientDetailViewModel.MealDetails != null){
                if (_dailyNutrientDetailViewModel.MealDetails.Count <= 0)
                {
                    LoadPerMealNutrientsBarChart(new MealDetailViewModel());
                }
                else
                {
                    if (hasSetClicked == false)
                    {
                        LoadPerMealNutrientsBarChart(new MealDetailViewModel());
                    }
                }
            }
            else
            {
                LoadPerMealNutrientsBarChart(new MealDetailViewModel());
            }
        }
        catch (Exception ex)
        {
            ShowAlertBottomSheet("Intialize Meal Tab", "An error occurred while initializing meal tab. please go back to previous page and try again.", "OK");
        }
        finally
        {

        }

    }

    #endregion

    #region [Methods :: EventHandlers :: Controls]

    private void Calendar_SelectedDateChanged(object sender, EventArgs e)
    {
        GoToDate();
    }

    private void DatePickerButton_Clicked(object sender, EventArgs e)
    {
        this.CalendarDXPopup.IsOpen = true;
    }

    private void DatePickerLeft_Clicked(object sender, EventArgs e)
    {
        GoToPreviousDate();
    }

    private void DatePickerRight_Clicked(object sender, EventArgs e)
    {
        GoToNextDate();
    }

    private void BackButton_Clicked(object sender, EventArgs e)
    {
        GoBack();
    }

    private void MealTapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        Label label = (Label)sender;
        MealDetailViewModel mealDetail = (MealDetailViewModel)label.BindingContext;

        if (mealDetail != null)
        {
            HandleMealTabTapped(mealDetail);
        }
    }

    private void LoaderGridTapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        return;
    }

    #endregion

    #region [Methods :: Tasks]

    private void GetDailyNutrientDetailViewModel(DailyNutrientDetails dailyNutrientDetails)
    {
        try
        {
            if (dailyNutrientDetails != null)
            {
                _dailyNutrientDetailViewModel = new DailyNutrientDetailViewModel();
                _dailyNutrientDetailViewModel.NutrientsIntakeViewItem = new NutrientsIntakeViewItem();

                _dailyNutrientDetailViewModel.SelectedDate = this.RequestedDate;

                if (dailyNutrientDetails.DailyNutrientOverview != null)
                {
                    _dailyNutrientDetailViewModel.NutrientsIntakeViewItem.TargetCalories = Convert.ToInt32(dailyNutrientDetails.DailyNutrientOverview.TargetCalories);
                    _dailyNutrientDetailViewModel.NutrientsIntakeViewItem.TotalCalories = dailyNutrientDetails.DailyNutrientOverview.CurrentCalories;
                    _dailyNutrientDetailViewModel.NutrientsIntakeViewItem.CaloriesCarbs = dailyNutrientDetails.DailyNutrientOverview.CrabsGram;
                    _dailyNutrientDetailViewModel.NutrientsIntakeViewItem.CaloriesProtein = dailyNutrientDetails.DailyNutrientOverview.ProteinGram;
                    _dailyNutrientDetailViewModel.NutrientsIntakeViewItem.CaloriesFat = dailyNutrientDetails.DailyNutrientOverview.FatGram;

                    _dailyNutrientDetailViewModel.NutrientsIntakeViewItem.ProteinPercentage = _dailyNutrientDetailViewModel.NutrientsIntakeViewItem.CaloriesProtein / _dailyNutrientDetailViewModel.NutrientsIntakeViewItem.TotalCalories;
                    _dailyNutrientDetailViewModel.NutrientsIntakeViewItem.CarbPercentage = _dailyNutrientDetailViewModel.NutrientsIntakeViewItem.CaloriesCarbs / _dailyNutrientDetailViewModel.NutrientsIntakeViewItem.TotalCalories;
                    _dailyNutrientDetailViewModel.NutrientsIntakeViewItem.FatPercentage = _dailyNutrientDetailViewModel.NutrientsIntakeViewItem.CaloriesFat / _dailyNutrientDetailViewModel.NutrientsIntakeViewItem.TotalCalories;

                }

                _dailyNutrientDetailViewModel.MealDetails = new ObservableCollection<MealDetailViewModel>();

                if (dailyNutrientDetails.MealDetails != null)
                {
                    foreach (MealDetails mealDetails in dailyNutrientDetails.MealDetails)
                    {
                        MealDetailViewModel mealDetailViewModel = new MealDetailViewModel();

                        mealDetailViewModel.MealName = mealDetails.MealName;
                        mealDetailViewModel.IsClickable = mealDetails.IsClickable;
                        mealDetailViewModel.TargetCalories = mealDetails.TargetCalories;
                        mealDetailViewModel.CurrentCalories = mealDetails.CurrentCalories;
                        mealDetailViewModel.ProteinGram = mealDetails.ProteinGram;
                        mealDetailViewModel.CarbsGram = mealDetails.CarbsGram;
                        mealDetailViewModel.FatGram = mealDetails.FatGram;

                        mealDetailViewModel.MealSpecificNutrientOverView = new ObservableCollection<DishItemViewModel>();

                        if (mealDetails.MealSpecificNutrientOverview != null)
                        {
                            foreach (DishDetails dishDetails in mealDetails.MealSpecificNutrientOverview)
                            {
                                DishItemViewModel dishItemViewModel = new DishItemViewModel();
                                dishItemViewModel.ImageUrl = dishDetails.ImageUrl;
                                dishItemViewModel.Name = dishDetails.Name;
                                dishItemViewModel.Active = dishDetails.Active;
                                dishItemViewModel.Carb = dishDetails.Carb;
                                dishItemViewModel.Sugar = dishDetails.Sugar;
                                dishItemViewModel.Fibre = dishDetails.Fibre;
                                dishItemViewModel.Protein = dishDetails.Protein;
                                dishItemViewModel.Fat = dishDetails.Fat;
                                dishItemViewModel.Calories = dishDetails.Calories;
                                dishItemViewModel.StarturatedFat = dishDetails.SaturatedFat;
                                dishItemViewModel.UnsaturatedFat = dishDetails.UnsaturatedFat;
                                dishItemViewModel.Servings = dishDetails.Servings;
                                dishItemViewModel.AminoAcid = 0;
                                dishItemViewModel.IsExpanded = false;
                                switch (dishDetails.Active)
                                {
                                    case true:

                                        dishItemViewModel.Status = "in progress";
                                        break;

                                    case false:

                                        dishItemViewModel.Status = string.Empty;
                                        break;
                                }
                                mealDetailViewModel.MealSpecificNutrientOverView.Add(dishItemViewModel);

                            }
                        }

                        _dailyNutrientDetailViewModel.MealDetails.Add(mealDetailViewModel);
                    }
                }

            }
        }
        catch (Exception ex)
        {
            //App.Current.MainPage.DisplayAlert("Retrieve Daily Nutrient Detail View Model", "An error occurred while retrieving daily nutrient detail view model", "OK");
        }
        finally
        {

        }
    }

    private async Task<DailyNutrientDetails> GetDailyNutrientDetails(DateTime dateTime)
    {
        try
        {
            DailyNutrientDetails dailyNutrientDetails = await NutritionApi.GetDailyNutrientDetails(dateTime);
            return dailyNutrientDetails;
        }
        catch(Exception ex)
        {
            throw;
        }
        finally
        {

        }
        
    }

    private async Task<List<NutrientsIntakeViewItem>> LoadTotalNutrientsBarChart()
    {
        try
        {
            List<NutrientsIntakeViewItem> nutrientsIntakeViewItems = new List<NutrientsIntakeViewItem>();

            nutrientsIntakeViewItems = await _nutrientsIntakeService.LoadTotalNutrientsBarChart(_dailyNutrientDetailViewModel.SelectedDate);

            TotalNutrientsBarChartDrawable totalNutrientsBarChartDrawable = new TotalNutrientsBarChartDrawable();


            foreach (var item in nutrientsIntakeViewItems)
            {
                totalNutrientsBarChartDrawable.ProteinToDisplay += item.ProteinToDisplay;
                totalNutrientsBarChartDrawable.CarbohydratesToDisplay += item.CarbohydratesToDisplay;
                totalNutrientsBarChartDrawable.FatToDisplay += item.FatToDisplay;
                totalNutrientsBarChartDrawable.TargetCalories += item.TargetCalories;
                _dailyNutrientDetailViewModel.NutrientsIntakeViewItem.TranscribedCalories = item.TranscribedCalories;
                _dailyNutrientDetailViewModel.NutrientsIntakeViewItem.TargetCalories = item.TargetCalories;

            }

            this.TotalNutrientsGraphicsView.Drawable = totalNutrientsBarChartDrawable;

            //_isFullDayChartLoading = false;

            Application.Current.MainPage.Dispatcher.Dispatch(() =>
            {
                this.TotalNutrientsGraphicsView.Invalidate();
            });

            return nutrientsIntakeViewItems;
        }
        catch (Exception ex)
        {
            return new List<NutrientsIntakeViewItem>();
        }
        finally
        {
            //_isFullDayChartLoading = false;
            //EnableUI();
        }
    }

    private void LoadPerMealNutrientsBarChart(MealDetailViewModel mealNutrientDetail)
    {
        try
        {
            double caloriesCarbs = 0;
            double caloriesFat = 0;
            double caloriesProtein = 0;
            double caloriesTotal = 0;
            double ratio = 0;

            if (mealNutrientDetail != null)
            {
                caloriesCarbs = mealNutrientDetail.CarbsGram * 4;
                caloriesFat = mealNutrientDetail.FatGram * 9;
                caloriesProtein = mealNutrientDetail.ProteinGram * 4;
                caloriesTotal = caloriesCarbs + caloriesFat + caloriesProtein;

                //carbsIntake = Double.Round(caloriesCarbs / caloriesTotal, 3);
                //fatIntake = Double.Round(caloriesFat / caloriesTotal, 3);
                //proteinIntake = Double.Round(caloriesProtein / caloriesTotal, 3);

                NutrientsPerMealBarChartDrawable nutrientsPerMealBarChartDrawable = new NutrientsPerMealBarChartDrawable();


                ratio = caloriesTotal / mealNutrientDetail.CurrentCalories;

                if (Double.IsInfinity(ratio) == false && Double.IsNaN(ratio) == false)
                {
                    nutrientsPerMealBarChartDrawable.ProteinToDisplay = (int)Math.Ceiling((decimal)(caloriesProtein / ratio));
                    nutrientsPerMealBarChartDrawable.CarbohydratesToDisplay = (int)Math.Ceiling((decimal)(caloriesCarbs / ratio));
                    nutrientsPerMealBarChartDrawable.FatToDisplay = (int)Math.Ceiling((decimal)(caloriesFat / ratio));
                }

                if (mealNutrientDetail.TargetCalories <= 0)
                {
                    nutrientsPerMealBarChartDrawable.TargetCalories = (int)mealNutrientDetail.CurrentCalories;
                }
                else
                {
                    nutrientsPerMealBarChartDrawable.TargetCalories = (int)mealNutrientDetail.TargetCalories;
                }

                this.MealCurrentCaloriesLabelSpan.Text = mealNutrientDetail.CurrentCalories+"";
                this.MealTargetCaloriesLabelSpan.Text = nutrientsPerMealBarChartDrawable.TargetCalories+"";

                this.NutrientsPerMealGraphicsView.Drawable = nutrientsPerMealBarChartDrawable;

            }
        }
        catch (Exception ex)
        {

        }
        finally
        {            
        }
    }

    private void LoadMealSpecificOverview(ObservableCollection<DishItemViewModel> MealSpecificNutrientOverView)
    {
        if (this.SpecificMealGrid != null)
        {
            if (this.SpecificMealGrid.Children.Count <= 0)
            {
                this.SpecificMealGrid.Add(_mealContentView);
            }           
        }

        if (_mealContentView.DishItemViewModels == null)
        {
            _mealContentView.DishItemViewModels = new ObservableCollection<DishItemViewModel>();
        }

        _mealContentView.DishItemViewModels.Clear();
        foreach (DishItemViewModel dishItem in MealSpecificNutrientOverView)
        {

            _mealContentView.DishItemViewModels.Add(dishItem);
        }
        
        _mealContentView.RefreshData();
    }

    private async void GoBack()
    {
        await Navigation.PopAsync();
    }

    public void SetDate(DateTime? DateInput)
    {
        string formattedDate = string.Empty;
        string numberSuffix = string.Empty;
        string monthShort = string.Empty;
        if (DateInput != null)
        {
            if (_dailyNutrientDetailViewModel != null)
            {
                _dailyNutrientDetailViewModel.SelectedDate = DateInput.GetValueOrDefault();
                monthShort = _dailyNutrientDetailViewModel.SelectedDate.ToString("MMM");
                numberSuffix = GetDayNumberSuffix(_dailyNutrientDetailViewModel.SelectedDate);
                formattedDate = string.Format("{0} {1}, {2}", _dailyNutrientDetailViewModel.SelectedDate.Day + numberSuffix,
                    monthShort, _dailyNutrientDetailViewModel.SelectedDate.DayOfWeek);

                this.DatePickerButton.Text = formattedDate;
            }
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

    private async void RefreshData()
    {
        if (_mealContentView != null)
        {
            if (_mealContentView.DishItemViewModels != null)
            {
                _mealContentView.DishItemViewModels.Clear();
            }
        }

        await IntializeData();
        IntializeControl();
        await LoadTotalNutrientsBarChart();

        if (_mealContentView != null)
        {
            _mealContentView.RefreshData();
        }
    }

    private void ShowAlertBottomSheet(string title, string message, string cancelMessage)
    {
        if (App.alertBottomSheetManager != null)
        {
            App.alertBottomSheetManager.ShowAlertMessage(title, message, cancelMessage);
        }
    }

    private void HandleMealTabTapped(MealDetailViewModel mealDetail)
    {
        if(mealDetail.IsClickable == false)
        {
            return;
        }

        if(_mealContentView != null)
        {
            if(_mealContentView.DataIntialized == false)
            {
                return;
            }
            else
            {

            }
        }

        foreach (MealDetailViewModel mealDetailViewModel in _dailyNutrientDetailViewModel.MealDetails)
        {
            mealDetailViewModel.IsClicked = false;
        }

        mealDetail.IsClicked = true;

        LoadPerMealNutrientsBarChart(mealDetail);
        LoadMealSpecificOverview(mealDetail.MealSpecificNutrientOverView);
    }

    private void GoToPreviousDate()
    {
        if (_dailyNutrientDetailViewModel != null)
        {
            this.RequestedDate = this.RequestedDate.AddDays(-1);
            this.Calendar.SelectedDate = this.RequestedDate;
            SetDate(this.Calendar.SelectedDate);
        }
    }

    private void GoToNextDate()
    {
        if (_dailyNutrientDetailViewModel != null)
        {
            this.RequestedDate = this.RequestedDate.AddDays(1);
            this.Calendar.SelectedDate = this.RequestedDate;
            SetDate(this.Calendar.SelectedDate);
        }
    }

    private void GoToDate()
    {
        this.CalendarDXPopup.IsOpen = false;
        SetDate(this.Calendar.SelectedDate);
        RefreshData();
    }

    #endregion

    #region [Public :: Properties]

    public DateTime RequestedDate { get; set; }

    #endregion

}