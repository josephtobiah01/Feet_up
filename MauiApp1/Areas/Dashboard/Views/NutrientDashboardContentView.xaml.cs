using MauiApp1.Areas.Dashboard.DataManager;
using MauiApp1.Areas.Dashboard.Resources.Drawables;
using MauiApp1.Areas.Dashboard.ViewModel;

namespace MauiApp1.Areas.Dashboard.Views;

public partial class NutrientDashboardContentView : ContentView
{

    #region [Fields]

    private NutrientsDataManager _nutrientsDataManager;

    #endregion

    #region [Methods :: EventHandlers :: Class]

    public NutrientDashboardContentView()
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
        _nutrientsDataManager = new NutrientsDataManager();

        LoadProteinBarChart();

        LoadCarbohydratesBarChart();

        LoadFatBarChart();
    }

    private void InitializeControl()
    {
        
    }

    #endregion

    #region [Methods :: EventHandlers :: Controls]

   

    #endregion

    #region [Methods :: Tasks]

 

    private void LoadProteinBarChart()
    {
        List<ProteinIntakeViewItem> proteinIntakeViewItems = _nutrientsDataManager.GetProteinIntake();

        ProteinIntakeBarChartDrawable proteinIntakeBarChartDrawable = new ProteinIntakeBarChartDrawable();
        proteinIntakeBarChartDrawable.ProteinIntakeViewItems = proteinIntakeViewItems;

        this.ProteinIntakeGraphicsView.Drawable = proteinIntakeBarChartDrawable;

        Application.Current.MainPage.Dispatcher.Dispatch(() =>
        {
            this.ProteinIntakeGraphicsView.Invalidate();
        });
    }

    private void LoadCarbohydratesBarChart()
    {
        List<CarbohydratesIntakeViewItem> carbohydratesIntakeViewItems = _nutrientsDataManager.GetCarbohydratesIntake();

        CarbohydratesIntakeBarChartDrawable carbohydratesIntakeBarChartDrawable = new CarbohydratesIntakeBarChartDrawable();
        carbohydratesIntakeBarChartDrawable.CarbohydratesIntakeViewItems = carbohydratesIntakeViewItems;

        this.CarbohydratesIntakeGraphicsView.Drawable = carbohydratesIntakeBarChartDrawable;

        Application.Current.MainPage.Dispatcher.Dispatch(() =>
        {
            this.CarbohydratesIntakeGraphicsView.Invalidate();
        });
    }

    private void LoadFatBarChart()
    {
        List<FatIntakeViewItem> fatIntakeViewItems = _nutrientsDataManager.GetFatIntake();

        FatIntakeBarChartDrawable fatIntakeBarChartDrawable = new FatIntakeBarChartDrawable();
        fatIntakeBarChartDrawable.FatIntakeViewItems = fatIntakeViewItems;

        this.FatIntakeGraphicsView.Drawable = fatIntakeBarChartDrawable;

        Application.Current.MainPage.Dispatcher.Dispatch(() =>
        {
            this.FatIntakeGraphicsView.Invalidate();
        });
    }


    #endregion


}