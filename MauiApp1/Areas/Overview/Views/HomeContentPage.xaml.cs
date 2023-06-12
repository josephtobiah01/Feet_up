using MauiApp1.Areas.Biodata.Views;
using MauiApp1.Areas.Dashboard.Views;
using MauiApp1.Business.DeviceServices;

namespace MauiApp1.Areas.Overview.Views;

public partial class HomeContentPage : ContentPage
{


    #region [Fields]

    #endregion

    #region [Methods :: EventHandlers :: Class]

    public HomeContentPage()
    {
        InitializeComponent();
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        InitializeControl();
    }

    private void ContentPage_Unloaded(object sender, EventArgs e)
    {

    }

    private void InitializeControl()
    {
        this.DashboardRadioButton.IsChecked = true;
    }

    #endregion

    #region [Methods :: EventHandlers :: Controls]

    private void FeedRadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        RadioButton radioButton = sender as RadioButton;
        HandleFeedRibbonButtonCheckChange(radioButton);
    }

    private void DashboardRadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        RadioButton radioButton = sender as RadioButton;
        HandleDashboardRibbonButtonCheckChange(radioButton);
    }

    private void BiodataRadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        RadioButton radioButton = sender as RadioButton;
        HandleBiodataRibbonButtonCheckChange(radioButton);
    }

    #endregion

    #region [Methods :: Tasks]

    private async void HandleFeedRibbonButtonCheckChange(RadioButton radioButton)
    {
        switch(radioButton.IsChecked) 
        {
            case true:

                //if (HTMLBridge.RefreshData != null)
                //{
                //    HTMLBridge.RefreshData.Invoke(this, null);
                //}   
                await Navigation.PopAsync();
                break;

            case false:

            break;

        }
    }

    private void HandleDashboardRibbonButtonCheckChange(RadioButton radioButton)
    {
        switch (radioButton.IsChecked)
        {
            case true:

                if (this.DashboardContentStackLayout != null)
                {
                    if (this.DashboardContentStackLayout.Children.Count <= 0)
                    {
                        this.DashboardContentStackLayout.Add(new OverviewDashboard());
                    }
                    this.DashboardContentStackLayout.IsVisible = true;
                }
                break;

            case false:

                if (this.DashboardContentStackLayout != null)
                {
                    this.DashboardContentStackLayout.IsVisible = false;
                }
                break;

        }
    }

    private void HandleBiodataRibbonButtonCheckChange(RadioButton radioButton)
    {
        switch (radioButton.IsChecked)
        {
            case true:

                if (this.BiodataContentStackLayout != null)
                {
                    if (this.BiodataContentStackLayout.Children.Count <= 0)
                    {
                        this.BiodataContentStackLayout.Add(new OverviewBiodata());
                    }
                    this.BiodataContentStackLayout.IsVisible = true;
                }    
                
                break;

            case false:

                if (this.BiodataContentStackLayout != null)
                {
                    this.BiodataContentStackLayout.IsVisible = false;
                }
                break;

        }
    }


    #endregion

   
}