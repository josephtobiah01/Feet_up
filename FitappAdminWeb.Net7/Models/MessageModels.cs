using DAOLayer.Net7.Chat;
using MessageApi.Net7.Models;

namespace FitappAdminWeb.Net7.Models
{
    public class RoomSelectViewModel
    {
        public User? CurrentUser { get; set; }
        public List<MsgRoom> Rooms { get; set; } = new List<MsgRoom>();

        public bool WithConcernsOnly { get; set; }
    }

    public class SpecificUserChatViewModel
    {
        public User? LoggedInUser { get; set; }
        public User? RoomOwner { get; set; }
        public MsgRoom? CurrentRoom { get; set; }
        public List<ReceivedMessage>? Messages { get; set; } = new List<ReceivedMessage>();

        public SendMessageChatModel Data { get; set; } = new SendMessageChatModel();
    }

    public class SendMessageChatModel
    {
        public long RoomId { get; set; }
        public string? Message { get; set; }
        public bool MarkAsHandled { get; set; }
        public string? ImageContentType { get; set; }
        public string? ImageContent { get; set; }
    }
}
