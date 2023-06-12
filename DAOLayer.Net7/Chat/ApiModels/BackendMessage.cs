using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLayer.Net7.Chat.ApiModels
{
    public class BackendMessage
    {
        public string MessageContent { get; set; }

        public long Fk_Sender_Id { get; set; }

        public long Fk_Reciever_Id { get; set; }

    }
}
