namespace MauiApp1.Areas.Supplement.Views;

public partial class ViewAddSupplementSuccessContentPage : ContentPage
{

    #region [Methods :: EventHandlers :: Class]

    public ViewAddSupplementSuccessContentPage()
	{
		InitializeComponent();
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        IntializeControl();
    }

    private void IntializeControl()
    {
        
    }

    #endregion

    #region [Methods :: EventHandlers :: Controls]

    private void DoneButton_Clicked(object sender, EventArgs e)
    {
        Close();
    }

    #endregion

    #region [Methods :: Tasks]

    private async void Close()
    {
        await Navigation.PopToRootAsync();
    }

    #endregion

}