namespace MessageApi.Net7.Models
{
    public class FrontendMessage
    {
        public string MessageContent { get; set; }

        public string FkFederatedUser { get; set; }

        public string? ImageFileContentType { get; set; }

        public string? ImageFileContent { get; set; }
    }
}
