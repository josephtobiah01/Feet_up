using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageApi.Net7
{
    public class RecievedMessage
    {
        public string MessageContent { get; set; }
        public DateTime TimeStamp { get; set; }
        public string UserName { get; set; }

        public bool IsUserMessage { get; set; }
       
    }
}
