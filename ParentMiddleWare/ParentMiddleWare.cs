using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using ParentMiddleWare.ApiModels;

namespace ParentMiddleWare
{
    public class MiddleWare
    {

        // PROD
        //public static string QueueConnectionString = "DefaultEndpointsProtocol=https;AccountName=airusercontentprod;AccountKey=FvaOigk3Xf1BmIeTwaLrdSTdFLUpJAD2IWUC1eTgiTDGE08/LW3kqTHtkzxy9y04TCANOhizLmy0+AStX1wKXQ==;EndpointSuffix=core.windows.net";
        //public static string BaseUrl = "https://fitapp-mainapi-prod.azurewebsites.net";
        //public static string FunctionUrl = "https://air-functions-prod.azurewebsites.net";
        //public const string GoogleMobileAuthenticationEndpoint = "https://security.ageinreverse.me/mobileauth/Google";
        //public const string AppConnectEndpoint = "https://security.ageinreverse.me/en-US/v1/REST";
        //public const string FitnessDataEndpoint = "https://fitnessdata.ageinreverse.me/en-US/v1/REST";

        //// PROD -- ALTERNATIVE
        //public static string QueueConnectionString = "DefaultEndpointsProtocol=https;AccountName=airusercontentprod;AccountKey=FvaOigk3Xf1BmIeTwaLrdSTdFLUpJAD2IWUC1eTgiTDGE08/LW3kqTHtkzxy9y04TCANOhizLmy0+AStX1wKXQ==;EndpointSuffix=core.windows.net";
        //public static string BaseUrl = "https://fitapp-mainapi-prod-alt.azurewebsites.net";
        //public static string FunctionUrl = "https://air-functions-prod.azurewebsites.net";
        //public const string GoogleMobileAuthenticationEndpoint = "https://security.ageinreverse.me/mobileauth/Google";
        //public const string AppConnectEndpoint = "https://security.ageinreverse.me/en-US/v1/REST";
        //public const string FitnessDataEndpoint = "https://fitnessdata.ageinreverse.me/en-US/v1/REST";

        //Staging
        //public const string GoogleMobileAuthenticationEndpoint = "https://security-staging.ageinreverse.me/mobileauth/Google";
        //public const string AppConnectEndpoint = "https://security-staging.ageinreverse.me/en-US/v1/REST";
        //public const string FitnessDataEndpoint = "https://fitnessdata-staging.ageinreverse.me/en-US/v1/REST";


        // TEST
        public static string QueueConnectionString = "DefaultEndpointsProtocol=https;AccountName=airusercontent;AccountKey=VnPO2V4eMHeaF00gAHgK9cgy+AmdxJ+S8e8VCaBddFkiHSg0xzXygicQR2Us+dvbdy1S4fvo0LeQ+ASts1VIqg==;EndpointSuffix=core.windows.net";
        public static string BaseUrl = "https://fitapp-mainapi-test.azurewebsites.net";
        public static string FunctionUrl = "https://air-functions-test.azurewebsites.net";
        //public const string GoogleMobileAuthenticationEndpoint = "https://security-development.ageinreverse.me/mobileauth/Google";
        //public const string AppConnectEndpoint = "https://security-development.ageinreverse.me/en-US/v1/REST";
        //public const string FitnessDataEndpoint = "https://fitnessdata-development.ageinreverse.me/en-US/v1/REST";


        //Integration
        //public const string GoogleMobileAuthenticationEndpoint = "https://security-integration.ageinreverse.me/mobileauth/Google";
        //public const string AppConnectEndpoint = "https://security-integration.ageinreverse.me/en-US/v1/REST";
        //public const string FitnessDataEndpoint = "https://fitnessdata-integration.ageinreverse.me/en-US/v1/REST";

        //Development
        public const string GoogleMobileAuthenticationEndpoint = "https://security-development.ageinreverse.me/mobileauth/Google";
        public const string AppConnectEndpoint = "https://security-development.ageinreverse.me/en-US/v1/REST";
        public const string FitnessDataEndpoint = "https://fitnessdata-development.ageinreverse.me/en-US/v1/REST";



        protected readonly static HttpClient _httpClient = new HttpClient();
        public static long UserID { get; set; }
        public static string FkFederatedUser { get; set; } = string.Empty;

        // user info
        public static string UserName { get; set; }
        public static string Email { get; set; }
        public static string Age { get; set; }
        public static string Height { get; set; }
        public static string Last_recorded_weight { get; set; }
        public static string App_version = "1.1";
      

        public static bool IsInit = false;
        public static int AutoSkippedTimeout = 1200;
        public static int NowLaterTime = 60;
        public static int OverDueTime = 0;
        public static bool ShowFavoriteMsg = true;

        public static long TestKitStatus = 1;
        /* if 0, dont show scan feedcard
         * 1, show 'scan'
         * 2, waiting for results
         * 3, show results
         */

        public const int DefaultEncryptionId = 200;
        public const int DeviceIntegrationEncryptionId = 300;
        

        public static bool UseSecuredStorage { get; set; }

        public static void Setup()
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", "8ed4497d-f8ac-44bc-a68b-c6cb2a2f13a4");
            _httpClient.DefaultRequestHeaders.Add("x-api-version", "1.0");
        }

        public static Dictionary<DateTime, long> DailyPlanId  = new Dictionary<DateTime, long>();

        public static void SetBaseUrl(String url)
        {
            BaseUrl = url;
         //   MiddleWare._httpClient = new HttpClient();
        }



        public static void SetShowFavoriteMsg(bool value)
        {

        }


        public static async Task InsertSetHistory(long fk_exercise_id, short SetNumber, string SetString)
        {
            try
            {
                using (var response = await _httpClient.PostAsJsonAsync(string.Format("{0}/api/Exercise/InsertSetHistory", BaseUrl), new UserSetHistory { FkFederatedUser = FkFederatedUser, ExerciseTypeId = fk_exercise_id, SetNumber = SetNumber, SetString = SetString }))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
    //                var _result = JsonConvert.DeserializeObject<List<UserSetHistory>>(apiResponse);
                }
            }
            catch { }
        }

        public static async Task<List<UserSetHistory>> GetSetHistory()
        {
            try
            {
                using (var response = await _httpClient.GetAsync(string.Format("{0}/api/Exercise/GetSetHistory?FkFederatedUser={1}", BaseUrl, FkFederatedUser)))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var _result = JsonConvert.DeserializeObject<List<UserSetHistory>>(apiResponse);
                    return _result;

                }
            }
            catch 
            {
                return null;
            }
        }

        

        //protected async Task<HttpResponseMessage> Post(object json, string url)
        //{
        //    var inputJson = JsonConvert.SerializeObject(json);
        //    HttpContent inputContent = new StringContent(inputJson, Encoding.UTF8, "application/json");
        //    return await _httpClient.PostAsync(url, inputContent);
        //}

        //protected async Task<HttpResponseMessage> Get(object json, string url)
        //{
        //    var inputJson = Newtonsoft.Json.JsonConvert.SerializeObject(json);
        //    HttpContent inputContent = new StringContent(inputJson, Encoding.UTF8, "application/json");
        //    return await client.PostAsync(url, inputContent);
        //}
    }
}