using MauiApp1.Interfaces;
using MauiApp1.Pages.Chat;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace MauiApp1.Services
{
    public class SelectedImageService : ISelectedImageService, INotifyPropertyChanged
    {
        private string _selectedBottomSheetImage;

        private string _selectedChatContentImage;

        ViewChatDetailPage viewChatDetailPage = new ViewChatDetailPage();

        public event PropertyChangedEventHandler PropertyChanged;
        public string SelectedBottomSheetImage
        {
            get => _selectedBottomSheetImage;
            set
            {
                if (_selectedBottomSheetImage != value)
                {
                    _selectedBottomSheetImage = value;
                    OnPropertyChanged(nameof(SelectedBottomSheetImage));
                }
            }
        }

        public string SelectedChatContentImage
        {
            get => _selectedChatContentImage;
            set
            {
                if (_selectedChatContentImage != value)
                {
                    _selectedChatContentImage = value;
                    OnPropertyChanged(nameof(SelectedChatContentImage));
                }
            }
        }

        public event EventHandler<string> SelectedBottomSheetImageChanged;
        public event EventHandler<string> SelectedChatContentImageChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public async Task<string> UploadPhoto()
        {
            string uploadImage = "";
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Photos>();
            if (status != PermissionStatus.Denied || status != PermissionStatus.Unknown || status != PermissionStatus.Disabled)
            {
                ViewChatDetailPage._userMessageImage = "";
                //var result = await FilePicker.PickAsync(new PickOptions
                //{
                //    PickerTitle = "Select image(s) to send",
                //    FileTypes = FilePickerFileType.Images
                //});

                var result = await MediaPicker.Default.PickPhotoAsync();

                if (result == null)
                    return null;

                Microsoft.Maui.Graphics.IImage image = null;

                using (var stream = await result.OpenReadAsync())
                {
                    image = Microsoft.Maui.Graphics.Platform.PlatformImage.FromStream(stream);
                }

                int maxImageSize = 1600;
                float precision = 0.8f;
                byte[] bytes;

                if (image.Width > maxImageSize || image.Height > maxImageSize)
                {
                    image = image.Downsize(maxImageSize, true);
                }
                bytes = await image.AsBytesAsync(ImageFormat.Png, precision);


                uploadImage = Convert.ToBase64String(bytes);
                //  uploadImage = await ConvertStreamToBase64Async(stream);

               // SelectedBottomSheetImage = $"data:image/png;base64,{uploadImage}";

                SelectedBottomSheetImage = uploadImage;
            
                return SelectedBottomSheetImage;

                //selectedImageService.SelectedBottomSheetImage = SelectedImage;

                //await OpenGalleryAndBottomSheet.InvokeAsync();

            }

            else
            {
                var result = await Permissions.RequestAsync<Permissions.Photos>();

                if (result == PermissionStatus.Denied || result == PermissionStatus.Unknown || result != PermissionStatus.Disabled)
                {
                    await App.Current.MainPage.DisplayAlert("Required", "You must allow the permission to get your image", "Ok");
                }

                return null;
            }
        }

        private async Task<string> ConvertStreamToBase64Async(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                byte[] bytes = memoryStream.ToArray();
                return Convert.ToBase64String(bytes);
            }
        }

        protected virtual void OnSelectedBottomSheetImageChanged(string image)
        {
            SelectedBottomSheetImageChanged?.Invoke(this, image);
        }

        protected virtual void OnSelectedChatContentImageChanged(string image)
        {
          //  ViewChatDetailPage._userMessageImage = image;
            SelectedChatContentImageChanged?.Invoke(this, image);
        }

        public async Task SendMessage()
        {
            await viewChatDetailPage.SendMessage(image: SelectedBottomSheetImage);
        }
    }
}
