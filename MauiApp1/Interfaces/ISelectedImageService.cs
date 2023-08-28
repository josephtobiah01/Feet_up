using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Interfaces
{
    public interface ISelectedImageService
    {
        string SelectedBottomSheetImage { get; set; }
        string SelectedChatContentImage { get; set; }

        event EventHandler<string> SelectedBottomSheetImageChanged;

        event EventHandler<string> SelectedChatContentImageChanged;

        Task<string> UploadPhoto();

        Task SendMessage();
    }
}
