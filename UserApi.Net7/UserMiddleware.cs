﻿using System.Configuration;
using ParentMiddleWare;
using System.Net.Http.Json;
using Newtonsoft.Json;
using UserApi.Net7.Models;
using System.Numerics;
using ParentMiddleWare.Models;
using ParentMiddleWare.ApiModels;

namespace UserApi.Net7
{
    public class UserMiddleware : MiddleWare
    {

        public static async Task<EmTrainingSession> AddCustomTrainingSession()
        {
            try
            {
                DateTime date = DateTime.Now;
                DateTime edate = date.AddMinutes(30);
                string sString = string.Format("{0}/{1}/{2} {3}:{4}:{5}", date.Month, date.Day, date.Year, date.Hour, date.Minute, date.Second);
                string eString = string.Format("{0}/{1}/{2} {3}:{4}:{5}", edate.Month, edate.Day, edate.Year, edate.Hour, edate.Minute, edate.Second);
                //CreateCustomExercise
                using (var response = await _httpClient.PostAsync(string.Format("{0}/api/Exercise/CreateCustomExercise?UserID={1}&startDatetime={2}&EndDatetime={3}", BaseUrl, UserID, sString, eString), null))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var _result = JsonConvert.DeserializeObject<EmTrainingSession>(apiResponse);
                    return _result;
                }
            }
            catch(Exception e)
            {
                return null;
            }
        }



        public static async Task<bool> UpdateOffset()
        {
            using (var response = await _httpClient.PostAsJsonAsync(string.Format("{0}/api/User/UpdateOffset", BaseUrl), new SetOffsetModel { Offset = DateTimeOffset.Now, UserId = UserID }))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                var _result = JsonConvert.DeserializeObject<bool>(apiResponse);
                return _result;
            }
        }

        public static async Task<bool> RegisterDevice(string RegistrationId, string Platform)
        {
            if (Platform == "U") return true;
            using (var response = await _httpClient.PostAsync(string.Format("{0}/api/User/RegisterDevice?RegistrationId={1}&UserId={2}&Platform={3}", BaseUrl, RegistrationId, UserID, Platform), null))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                var _result = JsonConvert.DeserializeObject<bool>(apiResponse);
                return _result;
            }
        }

        public static async Task<bool> UnRegisterDevice(string RegistrationId, long UId, string Platform)
        {
            if (Platform == "U") return true;
            using (var response = await _httpClient.PostAsync(string.Format("{0}/api/User/UnRegisterDevice?RegistrationId={1}&UserId={2}&Platform={3}", BaseUrl, RegistrationId, UId, Platform), null))
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
            using (var response = await _httpClient.PostAsJsonAsync(string.Format("{0}/api/User/Login", BaseUrl), new LoginUserModel { Username = Username, Password = Password, Offset = DateTimeOffset.Now}))
            {
                var  apiResponse = await response.Content.ReadAsStringAsync();
                var _result = JsonConvert.DeserializeObject<UserOpResult>(apiResponse);
                return _result;
            }
        }

        public static async Task<bool> GetUserInfo()
        {
            try
            {
                using (var response = await _httpClient.PostAsJsonAsync(string.Format("{0}/api/User/GetUserInfo", BaseUrl), new GeneralApiModel { UserId = UserID }))
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    var _result = JsonConvert.DeserializeObject<UserOpResult>(apiResponse);

                    UserMiddleware.Age = _result.Age;
                    UserMiddleware.App_version = _result.App_version;
                    UserMiddleware.Email = _result.Email;
                    UserMiddleware.Last_recorded_weight = _result.Last_recorded_weight;
                    UserMiddleware.Height = _result.Height;
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }


        public static async Task<UserOpResult> LoginUserWINDOWS(string Username, string Password)
        {
            using (var response = await _httpClient.PostAsJsonAsync(string.Format("{0}/api/User/LoginWINDOWS", BaseUrl), new LoginUserModel { Username = Username, Password = Password, Offset = DateTimeOffset.Now }))
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var _result = JsonConvert.DeserializeObject<UserOpResult>(apiResponse);
                return _result;
            }
        }

        public static async Task<bool> SubmitBarcode(string BarcodeContents)
        {
            try
            {
                using (var response = await _httpClient.PostAsJsonAsync(string.Format("{0}/api/User/RegisterBarcode", BaseUrl), new GeneralApiModel { UserId = UserID, param1 = BarcodeContents }))
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    var _result = JsonConvert.DeserializeObject<bool>(apiResponse);
                    return _result;
                }
            }
            catch 
            {
                return false;
            }
        }

    }
}