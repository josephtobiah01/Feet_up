using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageApi.Net7
{
    public class FrontendMessage
    {
        public string MessageContent { get; set; }

        public long Fk_Sender_Id { get; set; }

    }
}
