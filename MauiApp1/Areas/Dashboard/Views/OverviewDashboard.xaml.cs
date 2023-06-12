using MauiApp1.Areas.Dashboard.DataManager;
using MauiApp1.Areas.Dashboard.Resources.Drawables;
using MauiApp1.Areas.Dashboard.ViewModel;

namespace MauiApp1.Areas.Dashboard.Views;

public partial class OverviewDashboard : ContentView
{

    #region [Fields]
    #endregion

    #region [Methods :: EventHandlers :: Class]

    public OverviewDashboard()
    {
        InitializeComponent();
    }

    private void ContentView_Loaded(object sender, EventArgs e)
    {
        InitializeData();
    }

    private void InitializeData()
    {
        NutrientsDataManager nutrientsDataManager = new NutrientsDataManager();

        List<ProteinIntakeViewItem> proteinIntakeViewItems = nutrientsDataManager.GetProteinIntake();

        ProteinIntakeBarChartDrawable proteinIntakeBarChartDrawable = new ProteinIntakeBarChartDrawable();
        proteinIntakeBarChartDrawable.ProteinIntakeViewItems = proteinIntakeViewItems;

        this.ProtienIntakeGraphicsView.Drawable = proteinIntakeBarChartDrawable;

        Application.Current.MainPage.Dispatcher.Dispatch(() =>
        {
            this.ProtienIntakeGraphicsView.Invalidate();
        });
    }


    #endregion

    #region [Methods :: EventHandlers :: Controls]
    #endregion

    #region [Methods :: Tasks]
    #endregion



}