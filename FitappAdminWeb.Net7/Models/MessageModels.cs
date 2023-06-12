using DAOLayer.Net7.Chat;

namespace FitappAdminWeb.Net7.Models
{
    public class RoomSelectViewModel
    {
        public User? CurrentUser { get; set; }
        public List<MsgRoom> RoomsWithConcerns { get; set; } = new List<MsgRoom>();
    }

    public class SpecificUserChatViewModel
    {
        public User? LoggedInUser { get; set; }
        public User? RoomOwner { get; set; }
        public MsgRoom? CurrentRoom { get; set; }

        public SendMessageChatModel Data { get; set; } = new SendMessageChatModel();
    }

    public class SendMessageChatModel
    {
        public long RoomId { get; set; }
        public string? Message { get; set; }
        public bool MarkAsHandled { get; set; }
    }
}
