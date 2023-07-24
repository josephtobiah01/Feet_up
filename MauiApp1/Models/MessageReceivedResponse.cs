using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Models
{
    public class MessageReceivedResponse
    {
        public string MessageImageContent { get; set; }
        public string MessageContent { get; set; }
        public DateTime TimeStamp { get; set; }
        public string UserName { get; set; }

        public bool IsUserMessage { get; set; }
    }
}
