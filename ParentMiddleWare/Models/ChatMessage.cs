using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParentMiddleWare.Models
{
    public  class ChatMessage
    {
        public long Id { get; set; }

        public string MessageContent { get; set; }

        public DateTime Timestamp { get; set; }

        public long FkUserSender { get; set; }

        public long FkRoomId { get; set; }

        public string UserName { get; set; }

        public virtual ChatRoom FkRoom { get; set; }
    }
}
