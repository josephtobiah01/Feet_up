using MauiApp1.Areas.Dashboard.ViewModel;
using System.Collections.ObjectModel;

namespace MauiApp1.Areas.Dashboard.Views;

public partial class SecondMealContentView : ContentView
{

    #region [Fields]

    private ObservableCollection<DishItemViewModel> _dishItemViewModels;

    private bool _dataInitialized = false;

    #endregion

    #region [Methods :: EventHandlers :: Class]

    public SecondMealContentView()
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
        if (this.DishItemViewModels == null)
        {
            this.DishItemViewModels = new List<DishItemViewModel>();
        }

        if (_dishItemViewModels == null)
        {
            _dishItemViewModels = new ObservableCollection<DishItemViewModel>();

            BindableLayout.SetItemsSource(this.DishDetailsVerticalStackLayout, _dishItemViewModels);
        }

        LoadDishItemViewModel();
    }

    private void InitializeControl()
    {

    }

    #endregion

    #region [Methods :: EventHandlers :: Controls]

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        Grid grid = sender as Grid;
        DishItemViewModel dishItemViewModel = grid.BindingContext as DishItemViewModel;

        if (dishItemViewModel != null)
        {
            switch (dishItemViewModel.Active)
            {
                case true:

                    CollapseOtherLayout(dishItemViewModel);
                    ExpandDishItemLayout(dishItemViewModel);
                    break;

                case false:
                default:
                    break;
            }
        }

    }

    private void ExpanderImageButton_Clicked(object sender, EventArgs e)
    {
        //ImageButton grid = sender as ImageButton;
        //DishItemViewModel dishItemViewModel = grid.BindingContext as DishItemViewModel;
        //CollapseOtherLayout(dishItemViewModel);
        //ExpandDishItemLayout(dishItemViewModel);
    }

    #endregion

    #region [Methods :: Tasks]

    private async void LoadDishItemViewModel()
    {
        try
        {
            _dataInitialized = false;
            this.LoadingActivityIndicator.IsVisible = true;
            _dishItemViewModels.Clear();
            await Task.Delay(2);
            List<DishItemViewModel> dishDetails = this.DishItemViewModels;
            foreach (DishItemViewModel dishDetail in dishDetails)
            {

                DishItemViewModel dishItemViewModel = GetDishItemViewModel(dishDetail);
                await Task.Delay(10);
                _dishItemViewModels.Add(dishItemViewModel);
            }
        }
        catch (Exception ex)
        {

        }
        finally
        {
            this.LoadingActivityIndicator.IsVisible = false;

            if (MealListDoneLoading != null)
            {
                MealListDoneLoading.Invoke(this, null);
            }

            _dataInitialized = true;

        }
    }

    //The GetDishItemViewModel will convert the model from the database to view model
    private DishItemViewModel GetDishItemViewModel(DishItemViewModel dishdetails)
    {
        double caloriesCarbs = 0;
        double caloriesFat = 0;
        double caloriesProtein = 0;
        double caloriesTotal = 0;
        double carbsIntake = 0;
        double fatIntake = 0;
        double proteinIntake = 0;

        DishItemViewModel dishItemViewModel = new DishItemViewModel();
        dishItemViewModel.ImageUrl = dishdetails.ImageUrl;
        dishItemViewModel.Name = dishdetails.Name;
        dishItemViewModel.Active = dishdetails.Active;
        dishItemViewModel.Carb = dishdetails.Carb;
        dishItemViewModel.Sugar = dishdetails.Sugar;
        dishItemViewModel.Fibre = dishdetails.Fibre;
        dishItemViewModel.Protein = dishdetails.Protein;
        dishItemViewModel.Fat = dishdetails.Fat;
        dishItemViewModel.Calories = dishdetails.Calories;
        dishItemViewModel.StarturatedFat = dishdetails.StarturatedFat;
        dishItemViewModel.UnsaturatedFat = dishdetails.UnsaturatedFat;
        dishItemViewModel.Servings = dishdetails.Servings;
        dishItemViewModel.AminoAcid = dishdetails.AminoAcid;
        dishItemViewModel.IsExpanded = dishdetails.IsExpanded;

        caloriesCarbs = dishdetails.Carb * 4;
        caloriesFat = dishItemViewModel.Fat * 9;
        caloriesProtein = dishItemViewModel.Protein * 4;
        caloriesTotal = caloriesCarbs + caloriesFat + caloriesProtein;

        carbsIntake = caloriesCarbs / caloriesTotal;
        fatIntake = caloriesFat / caloriesTotal;
        proteinIntake = caloriesProtein / caloriesTotal;

        dishItemViewModel.CarbsPercentageIntake = Double.Round(carbsIntake * 100,1, MidpointRounding.AwayFromZero);
        dishItemViewModel.FatPercentageIntake = Double.Round(fatIntake * 100,1, MidpointRounding.AwayFromZero);
        dishItemViewModel.ProteinPercentageIntake = Double.Round(proteinIntake * 100,1, MidpointRounding.AwayFromZero);

        return dishItemViewModel;
    }

    private void CollapseOtherLayout(DishItemViewModel dishItemViewModel)
    {
        if (_dishItemViewModels != null)
        {
            foreach (DishItemViewModel dishItem in _dishItemViewModels)
            {
                if (dishItem != dishItemViewModel)
                {
                    dishItem.IsExpanded = false;
                }
            }
        }
    }

    private void ExpandDishItemLayout(DishItemViewModel dishItemViewModel)
    {
        if (dishItemViewModel != null)
        {
            if (dishItemViewModel.IsExpanded == true)
            {
                dishItemViewModel.IsExpanded = false;
            }
            else
            {
                dishItemViewModel.IsExpanded = true;
            }
        }
    }


    #endregion

    #region [Public Methods :: Tasks]

    public void RefreshData()
    {
        if (_dataInitialized == true)
        {
            if (_dishItemViewModels != null)
            {
                _dishItemViewModels.Clear();
            }
            InitializeData();
        }
    }

    #endregion

    #region [Properties]

    public List<DishItemViewModel> DishItemViewModels;

    public event EventHandler MealListDoneLoading;

    public bool DataIntialized
    {
        get
        {
            return _dataInitialized;
        }
    }

    #endregion
}