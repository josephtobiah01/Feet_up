using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui;
using Microsoft.Maui.Dispatching;
using ZXing;
using ZXing.Net.Maui;

namespace MauiApp1.Areas.BarcodeScanning.Views
{
    public partial class BarcodeScannerContentPage
    {
        public string barcodescanningresult;
        private bool flashToggle = false;
        private bool scanToggle = false;

        public BarcodeScannerContentPage()
        {
            InitializeComponent();

            barcodeReader.Options = new BarcodeReaderOptions()
            {
                //if barcode scans regardless of camera orientation
                AutoRotate = true,
                //Formats to specify which formats are accepted or none if all are accepted
                Formats = ZXing.Net.Maui.BarcodeFormat.Code128,
                //Formats= BarcodeFormats.All,
                //Multiple to specify if multiple results are possible (will be saved in results collection list
                //TryHarder will *really* try harder to see whether there's a barcode (might see barcode in white noise)
                TryHarder = true,
                TryInverted = true
            };
        }
        protected void ContentPage_Loaded(object sender, EventArgs e)
        {
            barcodeReader.IsDetecting = true;
            barcodeReader.IsEnabled = true;
            flashToggle = false;
            flashButton.Source = "lightning.png";
            scanToggle = true;
            scanButton.Text = "Stop";
            barcodeReader.IsDetecting = true;
        }

        private void ContentPage_Unloaded(object sender, EventArgs e)
        {
            SetTorchLightOff();
        }

        private void BackButton_Clicked(object sender, EventArgs e)
        {
            Close();
        }
        private void ForwardButton_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage.Navigation.PushAsync(new BarcodeScanningResultContentPage(""));
        }
        private void FlashButton_Clicked(object sender, EventArgs e)
        {
            flashToggle = !flashToggle;
            if(flashToggle)
            {
                flashButton.Source = "lightningoff.png";
                barcodeReader.IsTorchOn = true;
            }
            else
            {
                flashButton.Source = "lightning.png";
                barcodeReader.IsTorchOn = false;
            }
        }
        private void ScanButton_Clicked(object sender, EventArgs e)
        {
            scanToggle = !scanToggle;
            if (scanToggle)
            {
                scanButton.Text = "Stop";
                barcodeReader.IsDetecting = true;
            }
            else
            {
                scanButton.Text = "Scan";
                barcodeReader.IsDetecting = false;
            }
        }


        public async void Close()
        {
            barcodeReader.IsDetecting = false;
            barcodeReader.IsEnabled = false;
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        private void CameraBarcodeReaderView_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
        {
            Dispatcher.Dispatch(() =>
            {
                SetTorchLightOff();
                barcodescanningresult = e.Results[0].Value.ToString();
                barcodeReader.IsDetecting = false;
                barcodeReader.IsEnabled = false;
                Application.Current.MainPage.Navigation.PushAsync(new BarcodeScanningResultContentPage(barcodescanningresult));
            });
        }


        private void SetTorchLightOff()
        {
            barcodeReader.IsTorchOn = false;
        }
       
    }
}