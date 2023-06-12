using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Areas.Chat.ViewModels
{
    public class PromotionViewModel
    {
        public string Icon { get; set; }
        public string Title { get; set; }
        public string Message { get; set; } 
        public string ImageUrl { get; set; }

        public int ImageHeight { get; set; }
        public int ImageWidth { get; set; }

        public string LinkUrl { get; set; } 

        public bool HasImageUrl { get; set; }
        public bool HasIconUrl { get; set; }
        public bool HasLinkUrl { get; set; }
    }
}
