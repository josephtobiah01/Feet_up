namespace MauiApp1.Areas.DeviceIntegration;

public partial class ViewDeviceIntegration : ContentPage
{
    #region [Fields]

    //private FitnessService _fitnessService;


    #endregion


    #region [Methods :: EventHandlers :: Class]

    public ViewDeviceIntegration()
    {
        InitializeComponent();
    }

    //private void ContentPage_Loaded(object sender, EventArgs e)
    //{
    //    InitializeData();
    //    InitializeControls();
    //}

    private void ContentPage_Unloaded(object sender, EventArgs e)
    {

    }

    #endregion


    #region [Methods :: EventHandlers :: Controls]

    private void DisconnectButton_Clicked(object sender, EventArgs e)
    {
        //DissconnectApplication();
    }

    #endregion


    #region [Methods :: Tasks]

    private void InitializeControls()
    {

    }

    private void RefreshControls()
    {
        //this.BindingContext = _fitnessService;
    }

    private void DissconnectApplication()
    {
        
    }

    

    #endregion


    #region [Properties]

    public Guid FitnessClientLinkedAccountId { get; set; }

    #endregion


}