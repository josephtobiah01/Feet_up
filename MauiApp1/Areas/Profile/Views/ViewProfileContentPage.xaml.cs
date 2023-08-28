using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Platform;
using DevExpress.Maui.Controls;
using MauiApp1.Areas.BarcodeScanning.Views;
using MauiApp1.Areas.Dashboard.ViewModel;
using Microsoft.AspNetCore.Components;

namespace MauiApp1.Areas.Profile.Views;

public partial class ViewProfileContentPage
{
    #region [Fields]

   

    #endregion

    #region [Methods :: EventHandlers :: Class]

    public ViewProfileContentPage()
    {
        InitializeComponent(); 
        this.rootComponent.Parameters =
          new Dictionary<string, object>
          {
                { "OpenCameraPermissionCallback", new EventCallback(null, OpenCameraPermissionPopup) }
          };
    }
    private async void RequestCameraPermissions(object sender, EventArgs e)
    {
        PermissionStatus status = await Permissions.RequestAsync<Permissions.Camera>();
        if (this.CameraPermissionPopup != null)
        {
            this.CameraPermissionPopup.State = DevExpress.Maui.Controls.BottomSheetState.Hidden;
        }
        if (status != PermissionStatus.Denied)
        {
            await Task.Delay(1000);
            await Application.Current.MainPage.Navigation.PushAsync(new BarcodeScannerContentPage());
        }
        else
        {
            GoToScanResultPage();
        }
    }
    private async void GoToScanResultPage()
    {
        await Application.Current.MainPage.Navigation.PushAsync(new BarcodeScanningResultContentPage(""));
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

        DisposeControls();
    }
    public void OpenCameraPermissionPopup()
    {
        this.CameraPermissionPopup.State = BottomSheetState.HalfExpanded;
    }
    public void CloseCameraPermissionPopup(object sender, EventArgs e)
    {
        this.CameraPermissionPopup.State = BottomSheetState.Hidden;
    }
    public async void GoStraightToScanResult(object sender, EventArgs e)
    {
        await Application.Current.MainPage.Navigation.PushAsync(new BarcodeScanningResultContentPage(""));
    }
    #endregion

    #region [Methods :: EventHandlers :: Controls]

    #endregion

    #region [Methods :: Tasks]   

    private void DisposeControls()
    {
        if(App.alertBottomSheetManager != null)
        {
            App.alertBottomSheetManager.ClearConfirmationBottomSheetActionEvents();
        }        
    }

    #endregion

    #region [Methods :: Public EventHandlers :: Controls]

    public event EventHandler ConnectAppSucceeded;

    #endregion

    
}