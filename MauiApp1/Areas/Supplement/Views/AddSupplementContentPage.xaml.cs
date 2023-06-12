namespace MauiApp1.Areas.Supplement.Views;

public partial class AddSupplementContentPage : ContentPage
{


    #region [Methods :: EventHandlers :: Class]

    public AddSupplementContentPage()
    {
        InitializeComponent();
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        InitializeData();
        InitializeControls();
    }

    private void InitializeData()
    {

    }

    private void InitializeControls()
    {
        Task task = Task.Run(async delegate
        {
            await Task.Delay(1000);
            

            MainThread.BeginInvokeOnMainThread(() =>
            {
                LoadAddSupplementFormContentView();
                this.LoadingActivityIndicator.IsVisible= false;
            });
        });
       
     

    }

    #endregion

    #region [Methods :: EventHandlers :: Controls]

    private void BackButton_Clicked(object sender, EventArgs e)
    {
        CloseDialog();
    }

    #endregion

    #region [Methods :: Tasks]

    private void LoadAddSupplementFormContentView()
    {
        AddSupplementFormContentView addSupplementFormContentPage = new AddSupplementFormContentView();
        Grid.SetRow(addSupplementFormContentPage, 3);
        this.GridRootLayout.Add(addSupplementFormContentPage);
    }

    private async void CloseDialog()
    {
        await Navigation.PopAsync();
    }
    #endregion

}