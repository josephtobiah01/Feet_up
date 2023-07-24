using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Platform;
using MauiApp1.Areas.Dashboard.ViewModel;

namespace MauiApp1.Areas.Profile.Views;

public partial class ViewProfileContentPage
{
    #region [Fields]

   

    #endregion

    #region [Methods :: EventHandlers :: Class]

    public ViewProfileContentPage()
    {
        InitializeComponent();
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {

    }

    private void ContentPage_Unloaded(object sender, EventArgs e)
    {
        CommunityToolkit.Maui.Core.Platform.StatusBar.SetColor(Color.FromArgb("#006272"));
        CommunityToolkit.Maui.Core.Platform.StatusBar.SetStyle(StatusBarStyle.LightContent);
        
        if(RazorHomeViewModel.isFromRazorHomeContentPage == true)
        {
            RazorHomeViewModel.isNavigatedToProfilePage = true;
        }
        
    }

    #endregion

    #region [Methods :: EventHandlers :: Controls]

    #endregion

    #region [Methods :: Tasks]   

    #endregion

    #region [Methods :: Public EventHandlers :: Controls]

    public event EventHandler ConnectAppSucceeded;

    #endregion

    
}