namespace MessageApi.Net7.Models
{
    public class ReceivedMessage
    {
        public string? MessageContent { get; set; }

        public DateTime TimeStamp { get; set; }

        public string UserName { get; set; }

        public bool IsUserMessage { get; set; }

        public string? ThumbnailImageUrl { get; set; }

        public string? RealImageUrl { get; set; }

    }
}
