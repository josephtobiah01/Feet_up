using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UserApi.Net7;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Core.Platform;

namespace MauiApp1.Areas.BarcodeScanning.Views
{
	public partial class BarcodeScanningResultContentPage
	{
        public string _barcodescanresult;
		public BarcodeScanningResultContentPage (string barcodescanresult)
		{
			InitializeComponent();
            if (barcodescanresult != null && barcodescanresult != "")
            {
                CodeEntry.Text = barcodescanresult;
                _barcodescanresult = barcodescanresult;
            }

        }
        private void BackButton_Clicked(object sender, EventArgs e)
        {
            Close();
        }
        private async void SendButton_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CodeEntry.Text)) return;
            try
            {
                bool successful = await UserApi.Net7.UserMiddleware.SubmitBarcode(CodeEntry.Text.Trim());

                if (successful)
                {
                    Close();
                }
                else
                {
                    //await DisplayAlert("Error", "Error submitting barcode scan results. Please check your internet connection and try again.", "OK");
                    ShowAlertBottomSheet("Error", "Error submitting barcode scan results. Please check your internet connection and try again.", "OK");
                }
            }
            catch
            {
                ShowAlertBottomSheet("Error", "Error submitting barcode scan results. Please check your internet connection and try again.", "OK");
            }
        }
        public async void Close()
        {
            //await Application.Current.MainPage.Navigation.PopAsync();
            await Application.Current.MainPage.Navigation.PopToRootAsync();
        }
        private async void OnCodeKeyboardFocus(object sender, FocusEventArgs e)
        {
#if IOS || ANDROID
            if (KeyboardExtensions.IsSoftKeyboardShowing(this.CodeEntry) == false)
            {
                await KeyboardExtensions.ShowKeyboardAsync(this.CodeEntry, new CancellationToken());

            }
            //this.RootScrollView.IsEnabled = true;
            if (this.VariableRow != null)
            {
                this.VariableRow.Height = new GridLength(1, GridUnitType.Star);
            }
#endif
        }
        private async void OnCodeKeyboardUnfocus(object sender, FocusEventArgs e)
        {
#if IOS || ANDROID
            if (KeyboardExtensions.IsSoftKeyboardShowing(this.CodeEntry) == true)
            {
                await KeyboardExtensions.HideKeyboardAsync(this.CodeEntry, new CancellationToken());
            }
            if (this.VariableRow != null)
            {
                this.VariableRow.Height = new GridLength(0, GridUnitType.Star);
            }
#endif
        }

        #region [Methods :: Tasks]

        private void ShowAlertBottomSheet(string title, string message, string cancelMessage)
        {
            if (App.alertBottomSheetManager != null)
            {
                App.alertBottomSheetManager.ShowAlertMessage(title, message, cancelMessage);
            }
        }


        #endregion
    }
}