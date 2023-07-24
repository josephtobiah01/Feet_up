using ImageApi.Net7;
using MauiApp1.Areas.Dashboard.Resources.Drawables;
using MauiApp1.Areas.Dashboard.TemporaryStubModel;
using MauiApp1.Areas.Dashboard.ViewModel;
using MauiApp1.Interfaces;
using ParentMiddleWare.NutrientModels;
using System.ComponentModel;
using System.Linq.Expressions;

namespace MauiApp1.Areas.Dashboard.Views;

public partial class NutrientsDailyIntakeContentPage : ContentPage, INotifyPropertyChanged
{
    #region Fields

    private INutrientsIntakeService _nutrientsIntakeService;
    private long _targetCalories;
    private double _currentCalories;
    private long _mealTargetCalories;
    private double _mealCurrentCalories;
    private bool _isFirstMealClicked;
    private bool _isSecondMealClicked;
    private bool _isThirdMealClicked;
    private bool _isOtherClicked;
    private DateTime _selectedDate;
    //public DateTime _dateSelected = DateTime.Now;
    private FirstMealContentView _firstMealContentView;
    private SecondMealContentView _secondMealContentView;
    private ThirdMealContentView _thirdMealContentView;
    private OtherMealContentView _otherMealContentView;
    private DailyNutrientDetailViewModel _dailyNutrientDetailViewModel;

    private bool _isFirstMealClickable = false;
    private bool _isSecondMealClickable = false;
    private bool _isThirdMealClickable = false;
    private bool _isOtherMealClickable = false;

    //For stop Loading
    private bool _isUserInterfaceLoading = false;
    private bool _isFullDayChartLoading = false;
    private bool _isMealChartLoading = false;
    private bool _isMealListLoading = false;

    #endregion Fields

    #region Properties

    public event PropertyChangedEventHandler PropertyChanged;

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

    public long TargetCalories
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

    public long MealTargetCalories
    {
        get => _mealTargetCalories;
        set
        {
            if (_mealTargetCalories != value)
            {
                _mealTargetCalories = value;
                OnPropertyChanged(nameof(MealTargetCalories));
            }
        }
    }

    public double MealCurrentCalories
    {
        get => _mealCurrentCalories;
        set
        {
            if (_mealCurrentCalories != value)
            {
                _mealCurrentCalories = value;
                OnPropertyChanged(nameof(MealCurrentCalories));
            }
        }
    }

    public bool IsFirstMealClicked
    {
        get => _isFirstMealClicked;
        set
        {
            if (_isFirstMealClicked != value)
            {
                _isFirstMealClicked = value;
                OnPropertyChanged(nameof(IsFirstMealClicked));
            }
        }
    }

    public bool IsSecondMealClicked
    {
        get => _isSecondMealClicked;
        set
        {
            if (_isSecondMealClicked != value)
            {
                _isSecondMealClicked = value;
                OnPropertyChanged(nameof(IsSecondMealClicked));
            }
        }
    }

    public bool IsThirdMealClicked
    {
        get => _isThirdMealClicked;
        set
        {
            if (_isThirdMealClicked != value)
            {
                _isThirdMealClicked = value;
                OnPropertyChanged(nameof(IsThirdMealClicked));
            }
        }
    }

    public bool IsOtherMealClicked
    {
        get => _isOtherClicked;
        set
        {
            if (_isOtherClicked != value)
            {
                _isOtherClicked = value;
                OnPropertyChanged(nameof(IsOtherMealClicked));
            }
        }
    }


    #endregion Properties

    #region Methods
    #region Constructor

    public NutrientsDailyIntakeContentPage(INutrientsIntakeService nutrientsIntakeService)
    {
        InitializeComponent();
        BindingContext = this;

        _nutrientsIntakeService = nutrientsIntakeService;
        _firstMealContentView = new FirstMealContentView();
        _secondMealContentView = new SecondMealContentView();
        _thirdMealContentView = new ThirdMealContentView();
        _otherMealContentView = new OtherMealContentView();

        _firstMealContentView.MealListDoneLoading += MealListChart_DoneLoading;
        _secondMealContentView.MealListDoneLoading += MealListChart_DoneLoading;
        _thirdMealContentView.MealListDoneLoading += MealListChart_DoneLoading;
        _otherMealContentView.MealListDoneLoading += MealListChart_DoneLoading;

        //IsFirstMealClicked = true;
        //IsSecondMealClicked = false;

        //this.FirstMealStack.IsVisible = true;
        //this.SecondMealStack.IsVisible = false;
        //this.ThirdMealStack.IsVisible = false;

        //FirstMealStack.Children.Add(_firstMealContentView);

        ////_firstMealContentView.RefreshData();
    }


    #endregion Constructor


    protected override void OnPropertyChanged(string propertyName)
    {
        try
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        catch (Exception ex)
        {

        }
    }

    private void BackButton_Clicked(object sender, EventArgs e)
    {
        GoBack();
    }


    private void FirstMealLabelTapped(object sender, TappedEventArgs e)
    {
        CheckMealIsDoneLoading();
        EnableUI();

        if (_isUserInterfaceLoading == true)
        {
            return;
        }

        if (_isFirstMealClickable == false)
        {
            return;
        }
        else
        {
            //if (IsFirstMealClicked)
            //    return;

            DisableUIOnMealLabelTap();

            IsFirstMealClicked = true;
            IsSecondMealClicked = false;
            IsThirdMealClicked = false;
            IsOtherMealClicked = false;

            this.FirstMealStack.IsVisible = true;
            this.SecondMealStack.IsVisible = false;
            this.ThirdMealStack.IsVisible = false;
            this.OtherMealStack.IsVisible = false;

            if (FirstMealStack.Children.Count > 0)
                return;

            FirstMealStack.Children.Add(_firstMealContentView);
            SecondMealStack.Children.Remove(_secondMealContentView);
            ThirdMealStack.Children.Remove(_thirdMealContentView);
            OtherMealStack.Children.Remove(_otherMealContentView);

            if (_dailyNutrientDetailViewModel != null)
            {
                if (_dailyNutrientDetailViewModel.MealDetails != null)
                {
                    if (_dailyNutrientDetailViewModel.MealDetails.Count > 0)
                    {
                        LoadPerMealNutrientsBarChart(_dailyNutrientDetailViewModel.MealDetails[0]);
                    }
                    else
                    {
                        //LoadPerMealNutrientsBarChart(new MealDetailViewModel());
                    }
                }
            }
        }
    }

    private void SecondMealLabelTapped(object sender, TappedEventArgs e)
    {
        CheckMealIsDoneLoading();
        EnableUI();

        if (_isUserInterfaceLoading == true)
        {
            return;
        }

        if (_isSecondMealClickable == false)
        {
            return;
        }
        else
        {
            //if (IsSecondMealClicked)
            //    return;

            DisableUIOnMealLabelTap();

            IsFirstMealClicked = false;
            IsSecondMealClicked = true;
            IsThirdMealClicked = false;
            IsOtherMealClicked = false;

            this.FirstMealStack.IsVisible = false;
            this.SecondMealStack.IsVisible = true;
            this.ThirdMealStack.IsVisible = false;
            this.OtherMealStack.IsVisible = false;

            if (SecondMealStack.Children.Count > 0)
                return;

            SecondMealStack.Children.Add(_secondMealContentView);
            FirstMealStack.Children.Remove(_firstMealContentView);
            ThirdMealStack.Children.Remove(_thirdMealContentView);
            OtherMealStack.Children.Remove(_otherMealContentView);

            if (_dailyNutrientDetailViewModel != null)
            {
                if (_dailyNutrientDetailViewModel.MealDetails != null)
                {
                    if (_dailyNutrientDetailViewModel.MealDetails.Count > 1)
                    {
                        LoadPerMealNutrientsBarChart(_dailyNutrientDetailViewModel.MealDetails[1]);
                    }
                    else
                    {
                        //LoadPerMealNutrientsBarChart(new MealDetailViewModel());
                    }
                }
            }
        }
    }

    private void ThirdMealLabelTapped(object sender, TappedEventArgs e)
    {
        CheckMealIsDoneLoading();
        EnableUI();

        if (_isUserInterfaceLoading == true)
        {
            return;
        }

        if (_isThirdMealClickable == false)
        {
            return;
        }
        else
        {
            //if (IsThirdMealClicked)
            //    return;

            DisableUIOnMealLabelTap();

            IsFirstMealClicked = false;
            IsSecondMealClicked = false;
            IsThirdMealClicked = true;
            IsOtherMealClicked = false;

            this.FirstMealStack.IsVisible = false;
            this.SecondMealStack.IsVisible = false;
            this.ThirdMealStack.IsVisible = true;
            this.OtherMealStack.IsVisible = false;

            if (ThirdMealStack.Children.Count > 0)
                return;

            ThirdMealStack.Children.Add(_thirdMealContentView);
            FirstMealStack.Children.Remove(_firstMealContentView);
            SecondMealStack.Children.Remove(_secondMealContentView);
            OtherMealStack.Children.Remove(_otherMealContentView);

            if (_dailyNutrientDetailViewModel != null)
            {
                if (_dailyNutrientDetailViewModel.MealDetails != null)
                {
                    if (_dailyNutrientDetailViewModel.MealDetails.Count > 2)
                    {
                        LoadPerMealNutrientsBarChart(_dailyNutrientDetailViewModel.MealDetails[2]);
                    }
                    else
                    {
                        //LoadPerMealNutrientsBarChart(new MealDetailViewModel());
                    }
                }
            }
        }
    }

    private void OtherMealLabelTapped(object sender, TappedEventArgs e)
    {
        CheckMealIsDoneLoading();
        EnableUI();

        if (_isUserInterfaceLoading == true)
        {
            return;
        }

        if (_isOtherMealClickable == false)
        {
            return;
        }
        else
        {
            //if (IsOtherMealClicked)
            //    return;

            DisableUIOnMealLabelTap();

            IsFirstMealClicked = false;
            IsSecondMealClicked = false;
            IsThirdMealClicked = false;
            IsOtherMealClicked = true;

            this.FirstMealStack.IsVisible = false;
            this.SecondMealStack.IsVisible = false;
            this.ThirdMealStack.IsVisible = false;
            this.OtherMealStack.IsVisible = true;

            if (OtherMealStack.Children.Count > 0)
                return;

            OtherMealStack.Children.Add(_otherMealContentView);
            ThirdMealStack.Children.Remove(_thirdMealContentView);
            FirstMealStack.Children.Remove(_firstMealContentView);
            SecondMealStack.Children.Remove(_secondMealContentView);

            if (_dailyNutrientDetailViewModel != null)
            {
                if (_dailyNutrientDetailViewModel.MealDetails != null)
                {
                    if (_dailyNutrientDetailViewModel.MealDetails.Count > 3)
                    {
                        LoadPerMealNutrientsBarChart(_dailyNutrientDetailViewModel.MealDetails[3]);
                    }
                    else
                    {
                        //LoadPerMealNutrientsBarChart(new MealDetailViewModel());
                    }
                }
            }
        }
    }


    public async Task<List<NutrientsIntakeViewItem>> LoadTotalNutrientsBarChart()
    {
        try
        {
            //this.LoaderGrid.IsVisible = true;
            //this.LoadingActivityIndicator.IsVisible = true;
            _isFullDayChartLoading = true;

            List<NutrientsIntakeViewItem> nutrientsIntakeViewItems = new List<NutrientsIntakeViewItem>();

            nutrientsIntakeViewItems = await _nutrientsIntakeService.LoadTotalNutrientsBarChart(SelectedDate);

            TotalNutrientsBarChartDrawable totalNutrientsBarChartDrawable = new TotalNutrientsBarChartDrawable();

            //totalNutrientsBarChartDrawable.ShowTargetLine = false;

            foreach (var item in nutrientsIntakeViewItems)
            {
                totalNutrientsBarChartDrawable.ProteinToDisplay += item.ProteinToDisplay;
                totalNutrientsBarChartDrawable.CarbohydratesToDisplay += item.CarbohydratesToDisplay;
                totalNutrientsBarChartDrawable.FatToDisplay += item.FatToDisplay;
                totalNutrientsBarChartDrawable.TargetCalories += item.TargetCalories;
                CurrentCalories = item.TranscribedCalories; /*item.ProteinToDisplay + item.CarbohydratesToDisplay + item.FatToDisplay;*/
                TargetCalories = item.TargetCalories;

            }

            this.TotalNutrientsGraphicsView.Drawable = totalNutrientsBarChartDrawable;

            _isFullDayChartLoading = false;

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
            _isFullDayChartLoading = false;
            EnableUI();
        }
    }


    #endregion Methods

    #region [Methods :: EventHandlers :: Class]

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        //this.LoaderGrid.IsVisible = true;
        //this.LoadingActivityIndicator.IsVisible = true;

        await IntializeData();
        IntializeControl();
        InitializeSelectedDate();

        //if (_firstMealContentView != null)
        //{
        //    _firstMealContentView.RefreshData();
        //}

        //if (_secondMealContentView != null)
        //{
        //    _secondMealContentView.RefreshData();
        //}

        //if (_thirdMealContentView != null)
        //{
        //    _thirdMealContentView.RefreshData();
        //}

        //if (_otherMealContentView != null)
        //{
        //    _otherMealContentView.RefreshData();
        //}
    }

    private async Task IntializeData()
    {
        try
        {
            DailyNutrientDetails dailyNutrientDetails = await GetDailyNutrientDetails(this.SelectedDate);
            GetDailyNutrientDetailViewModel(dailyNutrientDetails);

            if (_dailyNutrientDetailViewModel != null)
            {
                if (_dailyNutrientDetailViewModel.MealDetails != null)
                {
                    if (_dailyNutrientDetailViewModel.MealDetails.Count > 0 && _dailyNutrientDetailViewModel.MealDetails[0].IsClickAble)
                    {
                        LoadPerMealNutrientsBarChart(_dailyNutrientDetailViewModel.MealDetails[0]);
                    }
                    else if (_dailyNutrientDetailViewModel.MealDetails.Count > 1 && _dailyNutrientDetailViewModel.MealDetails[1].IsClickAble)
                    {
                        LoadPerMealNutrientsBarChart(_dailyNutrientDetailViewModel.MealDetails[1]);
                    }
                    else if (_dailyNutrientDetailViewModel.MealDetails.Count > 2 && _dailyNutrientDetailViewModel.MealDetails[2].IsClickAble)
                    {
                        LoadPerMealNutrientsBarChart(_dailyNutrientDetailViewModel.MealDetails[2]);
                    }
                    else if (_dailyNutrientDetailViewModel.MealDetails.Count > 3 && _dailyNutrientDetailViewModel.MealDetails[3].IsClickAble)
                    {
                        LoadPerMealNutrientsBarChart(_dailyNutrientDetailViewModel.MealDetails[3]);
                    }
                    else
                    {
                        LoadPerMealNutrientsBarChart(new MealDetailViewModel());
                    }
                }
            }
        }
        catch (Exception ex)
        {
            //await App.Current.MainPage.DisplayAlert("Retrieve Daily Nutrient Detail", "An error occurred while retrieving daily nutrient detail", "OK");
        }
        finally
        {

        }

    }

    private void IntializeControl()
    {

        bool hasFirstMeal = false;
        bool isFirstMealClickable = false;
        bool hasSecondMeal = false;
        bool isSecondMealClickable = false;
        bool hasThirdMeal = false;
        bool isThirdMealClickable = false;
        bool hasOtherMeal = false;
        bool isOtherMealClickable = false;

        this.FirstMealLabel.IsVisible = false;
        this.SecondMealLabel.IsVisible = false;
        this.ThirdMealLabel.IsVisible = false;
        this.OtherMealLabel.IsVisible = false;



        if (_dailyNutrientDetailViewModel != null)
        {
            if (_dailyNutrientDetailViewModel.MealDetails != null)
            {
                for (int index = 0; index < _dailyNutrientDetailViewModel.MealDetails.Count; index++)
                {
                    MealDetailViewModel mealDetailViewModel = _dailyNutrientDetailViewModel.MealDetails[index];
                    switch (index)
                    {
                        case 0:

                            this.FirstMealLabel.Text = mealDetailViewModel.MealName;
                            isFirstMealClickable = mealDetailViewModel.IsClickAble;
                            this.FirstMealLabel.IsVisible = true;
                            hasFirstMeal = true;
                            _firstMealContentView.DishItemViewModels = _dailyNutrientDetailViewModel.MealDetails[index].MealSpecificNutrientOverView;
                            break;

                        case 1:

                            this.SecondMealLabel.Text = mealDetailViewModel.MealName;
                            isSecondMealClickable = mealDetailViewModel.IsClickAble;
                            this.SecondMealLabel.IsVisible = true;
                            hasSecondMeal = true;
                            _secondMealContentView.DishItemViewModels = _dailyNutrientDetailViewModel.MealDetails[index].MealSpecificNutrientOverView;
                            break;

                        case 2:

                            this.ThirdMealLabel.Text = mealDetailViewModel.MealName;
                            isThirdMealClickable = mealDetailViewModel.IsClickAble;
                            this.ThirdMealLabel.IsVisible = true;
                            hasThirdMeal = true;
                            _thirdMealContentView.DishItemViewModels = _dailyNutrientDetailViewModel.MealDetails[index].MealSpecificNutrientOverView;
                            break;

                        case 3:

                            this.OtherMealLabel.Text = mealDetailViewModel.MealName;
                            isOtherMealClickable = mealDetailViewModel.IsClickAble;
                            this.OtherMealLabel.IsVisible = true;
                            hasOtherMeal = true;
                            _otherMealContentView.DishItemViewModels = _dailyNutrientDetailViewModel.MealDetails[index].MealSpecificNutrientOverView;
                            break;
                    }
                }
            }
        }


        IntializeMealTabClick(isFirstMealClickable, isSecondMealClickable, isThirdMealClickable, isOtherMealClickable);

        DisableFirstMealLabel(hasFirstMeal, isFirstMealClickable);
        DisableSecondMealLabel(hasSecondMeal, isSecondMealClickable);
        DisableThirdMealLabel(hasThirdMeal, isThirdMealClickable);
        DisableOtherMealLabel(hasOtherMeal, isOtherMealClickable);
    }

    #endregion

    #region [Methods :: Tasks]

    private void DisableFirstMealLabel(bool hasFirstMeal, bool isFirstMealClickable)
    {
        switch (hasFirstMeal)
        {
            case true:

                //_isFirstMealClickable = true;
                switch (IsFirstMealClicked)
                {
                    case true:
                        break;

                    case false:

                        this.FirstMealLabel.TextColor = Color.FromArgb("#85929B");
                        break;
                }
                break;

            case false:

                //this.FirstMealLabel.GestureRecognizers.Clear();
                //_isFirstMealClickable = false;
                //this.FirstMealLabel.TextColor = Color.FromArgb("#CED3D7");
                this.FirstMealLabel.TextColor = Color.FromArgb("#85929B");
                break;
        }

        switch (isFirstMealClickable)
        {
            case true:

                _isFirstMealClickable = true;
                break;

            case false:

                //this.FirstMealLabel.GestureRecognizers.Clear();
                _isFirstMealClickable = false;
                //this.FirstMealLabel.TextColor = Color.FromArgb("#85929B");
                this.FirstMealLabel.TextColor = Color.FromArgb("#CED3D7");
                break;
        }
    }

    private void DisableSecondMealLabel(bool hasSecondMeal, bool isSecondMealClickable)
    {
        switch (hasSecondMeal)
        {
            case true:

                _isSecondMealClickable = true;
                switch (IsSecondMealClicked)
                {
                    case true:
                        break;

                    case false:

                        this.SecondMealLabel.TextColor = Color.FromArgb("#85929B");
                        break;
                }

                break;

            case false:

                //this.SecondMealLabel.GestureRecognizers.Clear();
                //_isSecondMealClickable = false;
                //this.SecondMealLabel.TextColor = Color.FromArgb("#CED3D7");
                this.SecondMealLabel.TextColor = Color.FromArgb("#85929B");
                break;
        }

        switch (isSecondMealClickable)
        {
            case true:

                _isSecondMealClickable = true;
                break;

            case false:

                //this.SecondMealLabel.GestureRecognizers.Clear();
                _isSecondMealClickable = false;
                //this.SecondMealLabel.TextColor = Color.FromArgb("#85929B");
                this.SecondMealLabel.TextColor = Color.FromArgb("#CED3D7");
                break;
        }
    }

    private void DisableThirdMealLabel(bool hasThirdMeal, bool isThirdMealClickable)
    {
        switch (hasThirdMeal)
        {
            case true:

                //_isThirdMealClickable = true;
                switch (IsThirdMealClicked)
                {
                    case true:
                        break;

                    case false:

                        this.ThirdMealLabel.TextColor = Color.FromArgb("#85929B");
                        break;
                }
                break;

            case false:

                //this.ThirdMealLabel.GestureRecognizers.Clear();
                //_isThirdMealClickable = false;
                //this.ThirdMealLabel.TextColor = Color.FromArgb("#CED3D7");
                this.ThirdMealLabel.TextColor = Color.FromArgb("#85929B");
                break;
        }

        switch (isThirdMealClickable)
        {
            case true:

                _isThirdMealClickable = true;
                break;

            case false:

                //this.ThirdMealLabel.GestureRecognizers.Clear();
                _isThirdMealClickable = false;
                //this.ThirdMealLabel.TextColor = Color.FromArgb("#85929B");
                this.ThirdMealLabel.TextColor = Color.FromArgb("#CED3D7");
                break;
        }
    }

    private void DisableOtherMealLabel(bool hasOther, bool isOtherMealClickable)
    {
        switch (hasOther)
        {
            case true:

                //_isOtherMealClickable = true;
                switch (IsOtherMealClicked)
                {
                    case true:
                        break;

                    case false:

                        this.OtherMealLabel.TextColor = Color.FromArgb("#85929B");
                        break;
                }
                break;

            case false:

                //this.OtherMealLabel.GestureRecognizers.Clear();
                //_isOtherMealClickable = false;
                //this.OtherMealLabel.TextColor = Color.FromArgb("#CED3D7");
                this.OtherMealLabel.TextColor = Color.FromArgb("#85929B");
                break;
        }

        switch (isOtherMealClickable)
        {
            case true:

                _isOtherMealClickable = true;
                break;

            case false:

                //this.OtherMealLabel.GestureRecognizers.Clear();
                _isOtherMealClickable = false;
                //this.OtherMealLabel.TextColor = Color.FromArgb("#85929B");
                this.OtherMealLabel.TextColor = Color.FromArgb("#CED3D7");
                break;
        }

    }

    private void IntializeMealTabClick(bool isFirstMealClickable, bool isSecondMealClickable,
        bool isThirdMealClickable, bool isOtherMealClickable)
    {

        try
        {
            this.FirstMealStack.Children.Clear();
            this.SecondMealStack.Children.Clear();
            this.ThirdMealStack.Children.Clear();
            this.OtherMealStack.Children.Clear();

            if (isFirstMealClickable == true)
            {
                IsFirstMealClicked = true;
                IsSecondMealClicked = false;
                IsThirdMealClicked = false;
                IsOtherMealClicked = false;

                this.FirstMealStack.IsVisible = true;
                this.SecondMealStack.IsVisible = false;
                this.ThirdMealStack.IsVisible = false;
                this.OtherMealStack.IsVisible = false;

                this.FirstMealStack.Children.Add(_firstMealContentView);

            }
            else if (isSecondMealClickable == true)
            {
                IsFirstMealClicked = false;
                IsSecondMealClicked = true;
                IsThirdMealClicked = false;
                IsOtherMealClicked = false;

                this.FirstMealStack.IsVisible = false;
                this.SecondMealStack.IsVisible = true;
                this.ThirdMealStack.IsVisible = false;
                this.OtherMealStack.IsVisible = false;

                this.SecondMealStack.Children.Add(_secondMealContentView);
            }
            else if (isThirdMealClickable == true)
            {
                IsFirstMealClicked = false;
                IsSecondMealClicked = false;
                IsThirdMealClicked = true;
                IsOtherMealClicked = false;

                this.FirstMealStack.IsVisible = false;
                this.SecondMealStack.IsVisible = false;
                this.ThirdMealStack.IsVisible = true;
                this.OtherMealStack.IsVisible = false;

                this.ThirdMealStack.Children.Add(_thirdMealContentView);
            }
            else if (isOtherMealClickable == true)
            {
                IsFirstMealClicked = false;
                IsSecondMealClicked = false;
                IsThirdMealClicked = false;
                IsOtherMealClicked = true;

                this.FirstMealStack.IsVisible = false;
                this.SecondMealStack.IsVisible = false;
                this.ThirdMealStack.IsVisible = false;
                this.OtherMealStack.IsVisible = true;

                this.OtherMealStack.Children.Add(_otherMealContentView);
            }
            else
            {
                IsFirstMealClicked = false;
                IsSecondMealClicked = false;
                IsThirdMealClicked = false;
                IsOtherMealClicked = false;

                this.FirstMealStack.IsVisible = false;
                this.SecondMealStack.IsVisible = false;
                this.ThirdMealStack.IsVisible = false;
                this.OtherMealStack.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            App.Current.MainPage.DisplayAlert("Intialize Meal Tab", "An error occurred while initializing meal tab. please go back to previous page and try again.", "OK");
        }
        finally
        {

        }

    }

    public void LoadPerMealNutrientsBarChart(MealDetailViewModel mealNutrientDetail)
    {
        try
        {
            _isMealChartLoading = true;
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

                MealCurrentCalories = mealNutrientDetail.CurrentCalories;
                MealTargetCalories = nutrientsPerMealBarChartDrawable.TargetCalories;

                this.NutrientsPerMealGraphicsView.Drawable = nutrientsPerMealBarChartDrawable;

            }
        }
        catch (Exception ex)
        {

        }
        finally
        {
            _isMealChartLoading = false;
            EnableUI();
        }
    }

    private void GetDailyNutrientDetailViewModel(DailyNutrientDetails dailyNutrientDetails)
    {
        try
        {
            if (dailyNutrientDetails != null)
            {
                _dailyNutrientDetailViewModel = new DailyNutrientDetailViewModel();
                _dailyNutrientDetailViewModel.nutrientsIntakeViewItem = new NutrientsIntakeViewItem();

                if (dailyNutrientDetails.DailyNutrientOverview != null)
                {
                    _dailyNutrientDetailViewModel.nutrientsIntakeViewItem.TargetCalories = Convert.ToInt32(dailyNutrientDetails.DailyNutrientOverview.TargetCalories);
                    _dailyNutrientDetailViewModel.nutrientsIntakeViewItem.TotalCalories = dailyNutrientDetails.DailyNutrientOverview.CurrentCalories;
                    _dailyNutrientDetailViewModel.nutrientsIntakeViewItem.CaloriesCarbs = dailyNutrientDetails.DailyNutrientOverview.CrabsGram;
                    _dailyNutrientDetailViewModel.nutrientsIntakeViewItem.CaloriesProtein = dailyNutrientDetails.DailyNutrientOverview.ProteinGram;
                    _dailyNutrientDetailViewModel.nutrientsIntakeViewItem.CaloriesFat = dailyNutrientDetails.DailyNutrientOverview.FatGram;

                    _dailyNutrientDetailViewModel.nutrientsIntakeViewItem.ProteinPercentage = _dailyNutrientDetailViewModel.nutrientsIntakeViewItem.CaloriesProtein / _dailyNutrientDetailViewModel.nutrientsIntakeViewItem.TotalCalories;
                    _dailyNutrientDetailViewModel.nutrientsIntakeViewItem.CarbPercentage = _dailyNutrientDetailViewModel.nutrientsIntakeViewItem.CaloriesCarbs / _dailyNutrientDetailViewModel.nutrientsIntakeViewItem.TotalCalories;
                    _dailyNutrientDetailViewModel.nutrientsIntakeViewItem.FatPercentage = _dailyNutrientDetailViewModel.nutrientsIntakeViewItem.CaloriesFat / _dailyNutrientDetailViewModel.nutrientsIntakeViewItem.TotalCalories;

                }

                _dailyNutrientDetailViewModel.MealDetails = new List<MealDetailViewModel>();

                if (dailyNutrientDetails.MealDetails != null)
                {
                    foreach (MealDetails mealDetails in dailyNutrientDetails.MealDetails)
                    {
                        MealDetailViewModel mealDetailViewModel = new MealDetailViewModel();

                        mealDetailViewModel.MealName = mealDetails.MealName;
                        mealDetailViewModel.IsClickAble = mealDetails.IsClickable;
                        mealDetailViewModel.TargetCalories = mealDetails.TargetCalories;
                        mealDetailViewModel.CurrentCalories = mealDetails.CurrentCalories;
                        mealDetailViewModel.ProteinGram = mealDetails.ProteinGram;
                        mealDetailViewModel.CarbsGram = mealDetails.CarbsGram;
                        mealDetailViewModel.FatGram = mealDetails.FatGram;

                        mealDetailViewModel.MealSpecificNutrientOverView = new List<DishItemViewModel>();

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
    /*
    private void SetSelectedDateLabel()
    {
        if (this.SelectedDate.Date == DateTime.Now)
        {
            this.SelectedDatelabel.Text = "Today >";
        }
        else
        {
            this.SelectedDatelabel.Text = this.SelectedDate.Date.ToString("dddd, MMMM dd, yyyy") + " >";
        }

    }
    */
    #region DatePickerStuff
    private void InitializeSelectedDate()
    {
        SetDate(SelectedDate);
    }
    public void SetDate(DateTime? DateInput)
    {
        string formattedDate = string.Empty;
        string numberSuffix = string.Empty;
        string monthShort = string.Empty;
        if (DateInput != null)
        {
            //System.Diagnostics.Debug.WriteLine(DateInput.ToString());

            SelectedDate = DateInput.GetValueOrDefault();
            monthShort = SelectedDate.ToString("MMM");
            numberSuffix = GetDayNumberSuffix(SelectedDate);
            formattedDate = string.Format("{0} {1}, {2}", SelectedDate.Day + numberSuffix, monthShort, SelectedDate.DayOfWeek);

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
        CheckMealIsDoneLoading();
        EnableUI();

        if (_isUserInterfaceLoading == true)
        {
            return;
        }

        this.CalendarDXPopup.IsOpen = true;
    }
    private void DatePickerLeft_Clicked(object sender, EventArgs e)
    {
        CheckMealIsDoneLoading();
        EnableUI();

        if (_isUserInterfaceLoading == true)
        {
            return;
        }

        SelectedDate = SelectedDate.AddDays(-1);
        this.Calendar.SelectedDate = SelectedDate;
        SetDate(SelectedDate);

    }
    private void DatePickerRight_Clicked(object sender, EventArgs e)
    {
        CheckMealIsDoneLoading();
        EnableUI();

        if (_isUserInterfaceLoading == true)
        {
            return;
        }

        SelectedDate = SelectedDate.AddDays(1);
        this.Calendar.SelectedDate = SelectedDate;
        SetDate(SelectedDate);

    }
    public void DateSelected(object sender, EventArgs e)
    {
        this.CalendarDXPopup.IsOpen = false;
        SetDate(this.Calendar.SelectedDate);
        RefreshData();
    }

    #endregion



    private async void GoBack()
    {
        await Navigation.PopAsync();

    }


    private void DisableUIOnMealLabelTap()
    {
        //this.LoaderGrid.IsVisible = true;
        //this.LoadingActivityIndicator.IsVisible = true;
        _isUserInterfaceLoading = true;
        _isFullDayChartLoading = false;
        //_isMealChartLoading = true;
        _isMealListLoading = true;
        this.CalendarDXPopup.IsEnabled = false;
    }

    private void EnableUI()
    {

        if (_isFullDayChartLoading == false && _isMealChartLoading == false && _isMealListLoading == false)
        {
            //this.LoaderGrid.IsVisible = false;
            //this.LoadingActivityIndicator.IsVisible = false;
            _isUserInterfaceLoading = false;
            this.CalendarDXPopup.IsEnabled = true;
        }
    }

    private void CheckMealIsDoneLoading()
    {
        if (IsFirstMealClicked == true)
        {
            if (_firstMealContentView.DataIntialized == true)
            {
                _isMealListLoading = false;
            }
            else
            {
                return;
            }
        }

        if (IsSecondMealClicked == true)
        {
            if (_secondMealContentView.DataIntialized == true)
            {
                _isMealListLoading = false;
            }
            else
            {
                return;
            }
        }

        if (IsThirdMealClicked == true)
        {
            if (_thirdMealContentView.DataIntialized == true)
            {
                _isMealListLoading = false;
            }
            else
            {
                return;
            }
        }

        if (IsOtherMealClicked == true)
        {
            if (_otherMealContentView.DataIntialized == true)
            {
                _isMealListLoading = false;
            }
            else
            {
                return;
            }
        }
    }

    private void MealListChart_DoneLoading(object sender, EventArgs e)
    {
        CheckMealIsDoneLoading();
        EnableUI();
    }

    #endregion

    #region [Public Methods :: Tasks]

    public async void RefreshData()
    {
        if (_firstMealContentView != null)
        {
            if (_firstMealContentView.DishItemViewModels != null)
            {
                _firstMealContentView.DishItemViewModels.Clear();
            }
        }

        if (_secondMealContentView != null)
        {
            if (_secondMealContentView.DishItemViewModels != null)
            {
                _secondMealContentView.DishItemViewModels.Clear();
            }
        }

        if (_thirdMealContentView != null)
        {
            if (_thirdMealContentView.DishItemViewModels != null)
            {
                _thirdMealContentView.DishItemViewModels.Clear();
            }
        }

        if (_otherMealContentView != null)
        {
            if (_otherMealContentView.DishItemViewModels != null)
            {
                _otherMealContentView.DishItemViewModels.Clear();
            }
        }

        await IntializeData();
        IntializeControl();
        await LoadTotalNutrientsBarChart();

        if (_firstMealContentView != null)
        {
            _firstMealContentView.RefreshData();
        }

        if (_secondMealContentView != null)
        {
            _secondMealContentView.RefreshData();
        }

        if (_thirdMealContentView != null)
        {
            _thirdMealContentView.RefreshData();
        }

        if (_otherMealContentView != null)
        {
            _otherMealContentView.RefreshData();
        }

    }

    #endregion


    #region [Method :: Private API Call : Stubs]

    //To Simulate Database Call
    private async Task<DailyNutrientDetails> GetDailyNutrientDetails(DateTime dateTime)
    {

        DailyNutrientDetails dailyNutrientDetails = await NutritionApi.GetDailyNutrientDetails(dateTime);
        return dailyNutrientDetails;
    }

    #endregion
}