using ChartApi.Net7;
using Newtonsoft.Json;
using ParentMiddleWare;
using ParentMiddleWare.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace MessageApi.Net7
{
    public class MessageApi : MiddleWare
    {

        public static async Task<List<PromotionChatmessage>> GetPromotionChat()
        {
            using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Chat/GetPromotionChat", BaseUrl)))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                if (!String.IsNullOrEmpty(apiResponse))
                {
                    return JsonConvert.DeserializeObject<List<PromotionChatmessage>>(apiResponse);
                }
                return null;
            }
        }

        public static async Task<List<RecievedMessage>> GetMessages(long UserId, DateTime fromDate)
        {
            string dateString = string.Format("{0}/{1}/{2} {3}:{4}:{5}", fromDate.Month, fromDate.Day, fromDate.Year, fromDate.Hour, fromDate.Minute, fromDate.Second);
            using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Chat/GetMessagesFrontend?UserId={1}&fromDate={2}", BaseUrl, UserId, dateString)))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                if (!String.IsNullOrEmpty(apiResponse))
                {
                    return JsonConvert.DeserializeObject<List<RecievedMessage>>(apiResponse);
                }
                return null;
            }
        }

        public static async Task<RecievedMessage> SendMessage(FrontendMessage message)
        {
            try
            {
                using (var response = await _httpClient.PostAsJsonAsync(string.Format("{0}/api/Chat/SendMessageFrontend", BaseUrl), message))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var _result = JsonConvert.DeserializeObject<RecievedMessage>(apiResponse);
                    if (_result != null)
                    {
                        await AddUnhandledFlag(message.Fk_Sender_Id);
                    }
                    return _result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static async Task AddUnhandledFlag(long userId)
        {
            try
            {
                using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Chat/AddUnhandledFlag?UserId={1}", BaseUrl, userId), null))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {

            }

        }
    }
}