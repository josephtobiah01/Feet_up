using ChartApi.Net7;
using MessageApi.Net7.Models;
using Newtonsoft.Json;
using ParentMiddleWare;
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

        public static async Task<List<ReceivedMessage>> GetMessages(string FkFederatedUser, DateTime fromDate)
        {
            string dateString = string.Format("{0}/{1}/{2} {3}:{4}:{5}", fromDate.Month, fromDate.Day, fromDate.Year, fromDate.Hour, fromDate.Minute, fromDate.Second);
            using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Chat/GetMessagesFrontend?FkFederatedUser={1}&fromDate={2}", BaseUrl, FkFederatedUser, dateString)))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                if (!String.IsNullOrEmpty(apiResponse))
                {
                    return JsonConvert.DeserializeObject<List<ReceivedMessage>>(apiResponse);
                }
                return null;
            }
        }

        public static async Task<ReceivedMessage> SendMessage(FrontendMessage message)
        {
            try
            {
                using (var response = await _httpClient.PostAsJsonAsync(string.Format("{0}/api/Chat/SendMessageFrontend", BaseUrl), message))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var _result = JsonConvert.DeserializeObject<ReceivedMessage>(apiResponse);
                    if (_result != null)
                    {
                        await AddUnhandledFlag(message.FkFederatedUser);
                    }
                    return _result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static async Task AddUnhandledFlag(string FkFederatedUser)
        {
            try
            {
                //using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Chat/AddUnhandledFlag?UserId={1}", BaseUrl, userId), null))
                using (var response = await _httpClient.PostAsJsonAsync(string.Format("{0}/api/Chat/AddUnhandledFlag", BaseUrl), FkFederatedUser))
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