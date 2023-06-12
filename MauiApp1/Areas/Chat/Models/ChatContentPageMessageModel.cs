using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace MauiApp1.Areas.Chat.Models
{
    public class IMessage
    {
        public IMessage(string text, DateTime date, string username, bool isusersent)
        {
            MessageText = text;
            TimeStamp = date;
            UserName = username;
            IsUserMessage = isusersent;
        }

        public string MessageText { get; set; }
        public DateTime TimeStamp { get; set; }
        public string UserName { get; set; }   // name of the user who wrote  the message
        public bool IsUserMessage { get; set; }   // if true, then its a message the mobile user wrote, else its a message from backend
    }
}