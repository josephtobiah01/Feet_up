namespace ChartApi.Net7
{
    public class PromotionChatmessage
    {
        public string Title { get; set; }
        public string? Icon { get; set; }
        public string? Message { get; set; }
        public string? ImageUrl { get; set; }
        public string? LinkUrl { get; set; }
        public DateTime DateSentUTC { get; set; }
    }
}