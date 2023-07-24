using FitnessData.Common;
using Newtonsoft.Json;
using System.Text;

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

//    private void InitializeData()
//    {
//        string fitnessServiceToken = EncryptDecrypt(Preferences.Default.Get("fitness_service", string.Empty), 200);

//        if (string.IsNullOrWhiteSpace(fitnessServiceToken) == true)
//        {

//#if ANDROID
//            _fitnessService = JsonConvert.DeserializeObject<FitnessService>(fitnessServiceToken);
//#endif
//        }
//        else
//        {

//        }

//        RefreshControls();
//    }

    private void InitializeControls()
    {

    }

    private void RefreshControls()
    {
        //this.BindingContext = _fitnessService;
    }

    private void DissconnectApplication()
    {
        //Preferences.Default.Set("fitness_service", string.Empty);
    }

    //public static string EncryptDecrypt(string szPlainText, int szEncryptionKey)
    //{
    //    StringBuilder szInputStringBuild = new StringBuilder(szPlainText);
    //    StringBuilder szOutStringBuild = new StringBuilder(szPlainText.Length);
    //    char Textch;
    //    for (int iCount = 0; iCount < szPlainText.Length; iCount++)
    //    {
    //        Textch = szInputStringBuild[iCount];
    //        Textch = (char)(Textch ^ szEncryptionKey);
    //        szOutStringBuild.Append(Textch);
    //    }
    //    return szOutStringBuild.ToString();
    //}

    #endregion


    #region [Properties]

    public Guid FitnessClientLinkedAccountId { get; set; }

    #endregion


}