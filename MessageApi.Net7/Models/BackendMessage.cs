namespace MessageApi.Net7.Models
{
    public class BackendMessage
    {
        public string? MessageContent { get; set; }

        public string SenderFkFederatedUser { get; set; } = string.Empty;
        public string ReceiverFkFederatedUser { get; set; } = string.Empty;

        public string? ImageFileContentType { get; set; }

        public string? ImageFileContent { get; set; }
    }
}
