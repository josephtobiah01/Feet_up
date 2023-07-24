using FitnessData.Client.Business;
using FitnessData.Common;
using FitnessData.Common.Data;
using MauiApp1.Areas.Dashboard.DataManager;
using MauiApp1.Areas.Dashboard.Resources.Drawables;
using MauiApp1.Areas.Dashboard.ViewModel;
using MauiApp1.Interfaces;
using MauiApp1.Services;

namespace MauiApp1.Areas.Dashboard.Views;

public partial class OverviewDashboard : ContentView
{

    #region [Fields]

    private NutrientsDataManager _nutrientsDataManager;

    #endregion

    #region [Methods :: EventHandlers :: Class]

    public OverviewDashboard()
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
        //LoginGoogleFitness();
    }

    private void InitializeControl()
    {
        SetNutrientsDashboard();
        ShowNutrientsDashboard();
    }

    #endregion

    #region [Methods :: EventHandlers :: Controls]

    private void AllButton_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        HandleSelectAllDashboardClick(button);
    }

    private void ExerciseButton_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        HandleSelectExerciseDashboardClick(button);
    }

    private void NutrientsButton_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        HandleSelectNutrientsDashboardClick(button);
    }

    private void SleepButton_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        HandleSelectSleepDashboardClick(button);
    }

    #endregion

    #region [Methods :: Tasks]

    private async void LoginGoogleFitness()
    {
        //GoogleAuthorizationManager googleAuthorizationManager = new GoogleAuthorizationManager();
        //FitnessService fitnessService = await googleAuthorizationManager.GetFitnessServiceAsync("me");



        //GoogleFitStepsCountDataManager googleFitStepsCountDataManager = new GoogleFitStepsCountDataManager(fitnessService);
        //List<StepsCountDataPointViewItem> stepsCountDataPoints = googleFitStepsCountDataManager.GetTotalStepsCountPerDay();
    }

    private void HandleSelectAllDashboardClick(Button button)
    {
        SelectAllDashboard(button);        
        HideExerciseDashboard();
        HideNutrientsDashboard();
        HideSleepDashboard();
        ShowAllDashboard();
    }

    private void HandleSelectExerciseDashboardClick(Button button)
    {
        SelectExerciseDashboard(button);
        HideAllDashboard();
        HideNutrientsDashboard();
        HideSleepDashboard();
        ShowExerciseDashboard();
    }

    private void HandleSelectNutrientsDashboardClick(Button button)
    {
        SelectNutrientsDashboard(button);
        HideAllDashboard();
        HideExerciseDashboard();
        HideSleepDashboard();
        ShowNutrientsDashboard();
    }

    private void HandleSelectSleepDashboardClick(Button button)
    {
        SelectSleepDashboard(button);
        HideAllDashboard();
        HideExerciseDashboard();
        HideNutrientsDashboard();
        ShowSleepDashboard();
    }

    private void SelectAllDashboard(Button button)
    {
        DashboardViewModel dashboardViewModel = (DashboardViewModel)button.BindingContext;
        dashboardViewModel.ShowAllDashboard = true;
        dashboardViewModel.ShowExerciseDashboard = false;
        dashboardViewModel.ShowNutrientsDashboard = false;
        dashboardViewModel.ShowSleepDashboard = false;
        dashboardViewModel.ShowHabitDashboard = false;
        dashboardViewModel.ShowMindfulnessDashboard = false;
        dashboardViewModel.ShowSupplementDashboard = false;
        dashboardViewModel.ShowFastingDashboard = false;
    }

    private void SelectExerciseDashboard(Button button)
    {
        DashboardViewModel dashboardViewModel = (DashboardViewModel)button.BindingContext;
        dashboardViewModel.ShowAllDashboard = false;
        dashboardViewModel.ShowExerciseDashboard = true;
        dashboardViewModel.ShowNutrientsDashboard = false;
        dashboardViewModel.ShowSleepDashboard = false;
        dashboardViewModel.ShowHabitDashboard = false;
        dashboardViewModel.ShowMindfulnessDashboard = false;
        dashboardViewModel.ShowSupplementDashboard = false;
        dashboardViewModel.ShowFastingDashboard = false;
    }

    private void SelectNutrientsDashboard(Button button)
    {
        DashboardViewModel dashboardViewModel = (DashboardViewModel)button.BindingContext;
        dashboardViewModel.ShowAllDashboard = false;
        dashboardViewModel.ShowExerciseDashboard = false;
        dashboardViewModel.ShowNutrientsDashboard = true;
        dashboardViewModel.ShowSleepDashboard = false;
        dashboardViewModel.ShowHabitDashboard = false;
        dashboardViewModel.ShowMindfulnessDashboard = false;
        dashboardViewModel.ShowSupplementDashboard = false;
        dashboardViewModel.ShowFastingDashboard = false;

    }

    private void SelectSleepDashboard(Button button)
    {
        DashboardViewModel dashboardViewModel = (DashboardViewModel)button.BindingContext;
        dashboardViewModel.ShowAllDashboard = false;
        dashboardViewModel.ShowExerciseDashboard = false;
        dashboardViewModel.ShowNutrientsDashboard = false;
        dashboardViewModel.ShowSleepDashboard = true;
        dashboardViewModel.ShowHabitDashboard = false;
        dashboardViewModel.ShowMindfulnessDashboard = false;
        dashboardViewModel.ShowSupplementDashboard = false;
        dashboardViewModel.ShowFastingDashboard = false;
    }

    private void SelectHabitDashboard(Button button)
    {
        DashboardViewModel dashboardViewModel = (DashboardViewModel)button.BindingContext;
        dashboardViewModel.ShowAllDashboard = false;
        dashboardViewModel.ShowExerciseDashboard = false;
        dashboardViewModel.ShowNutrientsDashboard = false;
        dashboardViewModel.ShowSleepDashboard = false;
        dashboardViewModel.ShowHabitDashboard = true;
        dashboardViewModel.ShowMindfulnessDashboard = false;
        dashboardViewModel.ShowSupplementDashboard = false;
        dashboardViewModel.ShowFastingDashboard = false;
    }

    private void SelectMindfulnessDashboard(Button button)
    {
        DashboardViewModel dashboardViewModel = (DashboardViewModel)button.BindingContext;
        dashboardViewModel.ShowAllDashboard = false;
        dashboardViewModel.ShowExerciseDashboard = false;
        dashboardViewModel.ShowNutrientsDashboard = false;
        dashboardViewModel.ShowSleepDashboard = false;
        dashboardViewModel.ShowHabitDashboard = false;
        dashboardViewModel.ShowMindfulnessDashboard = true;
        dashboardViewModel.ShowSupplementDashboard = false;
        dashboardViewModel.ShowFastingDashboard = false;
    }

    private void SelectSupplementDashboard(Button button)
    {
        DashboardViewModel dashboardViewModel = (DashboardViewModel)button.BindingContext;
        dashboardViewModel.ShowAllDashboard = false;
        dashboardViewModel.ShowExerciseDashboard = false;
        dashboardViewModel.ShowNutrientsDashboard = false;
        dashboardViewModel.ShowSleepDashboard = false;
        dashboardViewModel.ShowHabitDashboard = false;
        dashboardViewModel.ShowMindfulnessDashboard = false;
        dashboardViewModel.ShowSupplementDashboard = true;
        dashboardViewModel.ShowFastingDashboard = false;
    }

    private void SelectFastingDashboard(Button button)
    {
        DashboardViewModel dashboardViewModel = (DashboardViewModel)button.BindingContext;
        dashboardViewModel.ShowAllDashboard = false;
        dashboardViewModel.ShowExerciseDashboard = false;
        dashboardViewModel.ShowNutrientsDashboard = false;
        dashboardViewModel.ShowSleepDashboard = false;
        dashboardViewModel.ShowHabitDashboard = false;
        dashboardViewModel.ShowMindfulnessDashboard = false;
        dashboardViewModel.ShowSupplementDashboard = false;
        dashboardViewModel.ShowFastingDashboard = true;
    }

    private void SetNutrientsDashboard()
    {
        DashboardViewModel dashboardViewModel = (DashboardViewModel)this.BindingContext;
        dashboardViewModel.ShowAllDashboard = false;
        dashboardViewModel.ShowExerciseDashboard = false;
        dashboardViewModel.ShowNutrientsDashboard = true;
        dashboardViewModel.ShowSleepDashboard = false;
        dashboardViewModel.ShowHabitDashboard = false;
        dashboardViewModel.ShowMindfulnessDashboard = false;
        dashboardViewModel.ShowSupplementDashboard = false;
        dashboardViewModel.ShowFastingDashboard = false;
    }

    private void ShowAllDashboard()
    {
        if (this.AllStackLayout != null)
        {
            if (this.AllStackLayout.Children.Count <= 0)
            {
                this.AllStackLayout.Add(new AllDashboardContentView());
            }
            this.AllStackLayout.IsVisible = true;
        }
    }

    private void HideAllDashboard()
    {
        if (this.AllStackLayout != null)
        {
            this.AllStackLayout.IsVisible = false;
        }
    }

    private void ShowExerciseDashboard()
    {
        if (this.ExerciseStackLayout != null)
        {
            if (this.ExerciseStackLayout.Children.Count <= 0)
            {
                this.ExerciseStackLayout.Add(new ExerciseDashboardContentView());
            }
            this.ExerciseStackLayout.IsVisible = true;
        }
    }

    private void HideExerciseDashboard()
    {
        if (this.ExerciseStackLayout != null)
        {
            this.ExerciseStackLayout.IsVisible = false;
        }
    }

    private void ShowNutrientsDashboard()
    {
        INutrientsIntakeService nutrientsIntakeService = new NutrientsIntakeService();
        NutrientDashboardContentView nutrientDashboardContentView = new NutrientDashboardContentView(nutrientsIntakeService);
        if (this.NutrientsStackLayout != null)
        {
            if (this.NutrientsStackLayout.Children.Count <= 0)
            {
                this.NutrientsStackLayout.Add(nutrientDashboardContentView);
            }
            this.NutrientsStackLayout.IsVisible = true;
        }
    }

    private void HideNutrientsDashboard()
    {
        if (this.NutrientsStackLayout != null)
        {
            this.NutrientsStackLayout.IsVisible = false;
        }
    }

    private void ShowSleepDashboard()
    {
        if (this.SleepStackLayout != null)
        {
            if (this.SleepStackLayout.Children.Count <= 0)
            {
                this.SleepStackLayout.Add(new SleepDashboardContentView());
            }
            this.SleepStackLayout.IsVisible = true;
        }
    }

    private void HideSleepDashboard()
    {
        if (this.SleepStackLayout != null)
        {
            this.SleepStackLayout.IsVisible = false;
        }
    }


    #endregion

}