using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParentMiddleWare.Models
{
    public  class ChatRoom
    {
        public long Id { get; set; }

        public string RoomName { get; set; }

        public virtual ICollection<ChatMessage> MsgMessage { get; } = new List<ChatMessage>();
    }
}
