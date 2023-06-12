namespace FitappAdminWeb.Net7.Classes.DTO
{
    public class ReceivedMessage_DTO
    {
        public string MessageContent { get; set; }
        public DateTime TimeStamp { get; set; }
        public string UserName { get; set; }
        public bool isUserMessage { get; set; }
    }
}
