using System.Configuration;
using ParentMiddleWare;
using System.Net.Http.Json;
using Newtonsoft.Json;
using UserApi.Net7.Models;
using System.Numerics;

namespace UserApi.Net7
{
    public class UserMiddleware : MiddleWare
    {

        public static async Task<bool> RegisterDevice(string RegistrationId)
        {
            using (var response = await _httpClient.PostAsync(string.Format("{0}/api/User/RegisterDevice?RegistrationId={1}&UserId={2}", BaseUrl, RegistrationId, UserID), null))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                var _result = JsonConvert.DeserializeObject<bool>(apiResponse);
                return _result;
            }
        }

        public static async Task<bool> UnRegisterDevice(string RegistrationId, long UId)
        {
            using (var response = await _httpClient.PostAsync(string.Format("{0}/api/User/UnRegisterDevice?RegistrationId={1}&UserId={2}", BaseUrl, RegistrationId, UId), null))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                var _result = JsonConvert.DeserializeObject<bool>(apiResponse);
                return _result;
            }
        }

        public static async Task<UserOpResult> CreateUser(string Username, string Email, string Password)
        {
            using (var response = await _httpClient.PostAsJsonAsync(string.Format("{0}/api/User/CreateUser", BaseUrl), new CreateUserModel { Username = Username, Email = Email, Password = Password }))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                var _result = JsonConvert.DeserializeObject<UserOpResult>(apiResponse);
                return _result;
            }
        }

        //public static async Task<UserOpResult> LoginUser(string Username, string Password)
        //{

        //    using (var response = await _httpClient.PostAsJsonAsync(string.Format("{0}/api/User/Login", BaseUrl), new CreateUserModel { Username = Username, Password = Password }))
        //    {
        //        string apiResponse = await response.Content.ReadAsStringAsync();
        //        var _result = JsonConvert.DeserializeObject<UserOpResult>(apiResponse);
        //        return _result;
        //    }
        //}


        public static async Task<List<UserOpResult>> GetAllUsers()
        {

            using (var response = await _httpClient.GetAsync(string.Format("{0}/api/User/GetAll", BaseUrl)))
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var _result = JsonConvert.DeserializeObject<List<UserOpResult>>(apiResponse);
                return _result;
            }
        }

        public static async Task<UserOpResult> LoginUser(string Username, string Password)
        {

            using (var response = await _httpClient.PostAsJsonAsync(string.Format("{0}/api/User/Login", BaseUrl), new CreateUserModel { Username = Username, Password = Password }))
            {
                var  apiResponse = await response.Content.ReadAsStringAsync();
                var _result = JsonConvert.DeserializeObject<UserOpResult>(apiResponse);
                return _result;
            }
        }

        public static async Task<bool> SubmitBarcode(string BarcodeContents)
        {
            return true;
        }

    }
}