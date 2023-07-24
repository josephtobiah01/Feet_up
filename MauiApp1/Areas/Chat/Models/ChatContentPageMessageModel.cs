using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace MauiApp1.Areas.Chat.Models
{
    public class IMessage
    {
        public IMessage(DateTime date, string username, bool isusersent, string? text, string? image = null)
        {
            MessageImage = image;
            MessageText = text;
            TimeStamp = date;
            UserName = username;
            IsUserMessage = isusersent;
        }

        
        public string MessageImage { get; set; }
        public string MessageText { get; set; }
        public DateTime TimeStamp { get; set; }
        public string UserName { get; set; }   // name of the user who wrote  the message
        public bool IsUserMessage { get; set; }   // if true, then its a message the mobile user wrote, else its a message from backend
    }

    //public class ImageMessage
    //{
    //    public ImageMessage(FileResult image, DateTime date, string userName, bool isUserSent)
    //    {
    //        MessageImage = image;
    //        TimeStamp = date;
    //        UserName = userName;
    //        IsUserMessage = isUserSent;
    //    }

    //    public FileResult MessageImage { get; set; }
    //    public DateTime TimeStamp { get; set; }
    //    public string UserName { get; set; }   // name of the user who wrote  the message
    //    public bool IsUserMessage { get; set; }
    //}
}