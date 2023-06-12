using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UserApi.Net7;
using System.Threading.Tasks;


namespace MauiApp1.Areas.BarcodeScanning.Views
{
	public partial class BarcodeScanningResultContentPage
	{
        public string _barcodescanresult;
		public BarcodeScanningResultContentPage (string barcodescanresult)
		{
			InitializeComponent();
            if (barcodescanresult != null && barcodescanresult!="")
            {
                myChatMessage.Text = barcodescanresult;
                _barcodescanresult = barcodescanresult;
            }

        }
        private void BackButton_Clicked(object sender, EventArgs e)
        {
            Close();
        }
        private async void SendButton_Clicked(object sender, EventArgs e)
        {
            bool successful = await UserApi.Net7.UserMiddleware.SubmitBarcode(_barcodescanresult);
            if (successful)
            {
                Close();
            }
            else
            {
                await DisplayAlert("Error", "Error submitting barcode scan results. Please check your internet connection and try again.", "OK");
            }
        }
        public async void Close()
        {
            //await Application.Current.MainPage.Navigation.PopAsync();
            await Application.Current.MainPage.Navigation.PopToRootAsync();
        }

    }
}