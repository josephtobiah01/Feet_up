using System.Text;
using Newtonsoft.Json;

namespace ParentMiddleWare
{
    public class MiddleWare
    {

        protected readonly static HttpClient _httpClient = new HttpClient();
        public static string QueueConnectionString = "DefaultEndpointsProtocol=https;AccountName=airusercontent;AccountKey=VnPO2V4eMHeaF00gAHgK9cgy+AmdxJ+S8e8VCaBddFkiHSg0xzXygicQR2Us+dvbdy1S4fvo0LeQ+ASts1VIqg==;EndpointSuffix=core.windows.net";
        public static long UserID { get; set; }
        public static string UserName { get; set; }

        public static string BaseUrl = "https://fitapp-mainapi-test.azurewebsites.net";
        // public static string FunctionUrl = "https://air-functions-prod.azurewebsites.net";

        // public static string BaseUrl = "https://fitapp-mainapi-dev.azurewebsites.net";


          // public static string BaseUrl = "https://localhost:7174";

        public static bool IsInit = false;
        public static int AutoSkippedTimeout = 1200;
        public static int NowLaterTime = 60;
        public static int OverDueTime = 30;
        public static bool ShowFavoriteMsg = true;

        public static long TestKitStatus = 1;
        /* if 0, dont show scan feedcard
         * 1, show 'scan'
         * 2, waiting for results
         * 3, show results
         */

        public static Dictionary<DateTime, long> DailyPlanId  = new Dictionary<DateTime, long>();

        public static void SetBaseUrl(String url)
        {
            BaseUrl = url;
         //   MiddleWare._httpClient = new HttpClient();
        }

        public static void SetShowFavoriteMsg(bool value)
        {

        }


        protected async Task<HttpResponseMessage> Post(object json, string url)
        {
            var inputJson = JsonConvert.SerializeObject(json);
            HttpContent inputContent = new StringContent(inputJson, Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(url, inputContent);
        }

        //protected async Task<HttpResponseMessage> Get(object json, string url)
        //{
        //    var inputJson = Newtonsoft.Json.JsonConvert.SerializeObject(json);
        //    HttpContent inputContent = new StringContent(inputJson, Encoding.UTF8, "application/json");
        //    return await client.PostAsync(url, inputContent);
        //}
    }
}