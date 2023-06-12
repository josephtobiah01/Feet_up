using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MauiApp1.Areas.Chat.Models;

namespace MauiApp1.Areas.Chat.Resources
{

    public class ChatContentPageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SenderTemplate { get; set; }
        public DataTemplate ReceiverTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return ((IMessage)item).IsUserMessage ? SenderTemplate : ReceiverTemplate;
        }
    }
}