using MuhdoApi.Net7.Model;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace MuhdoApi.Net7
{
    public class MuhdoMiddleware
    {
        protected readonly static HttpClient _httpClient = new HttpClient();
        protected string BaseUrl = "https://dev-api.muhdo.com/external-v3/";
        protected string Username = "AgeInReverse";
        protected string Password = "Age!nReVer$e492";

        protected async Task<HttpResponseMessage> Post(object json, string url)
        {
            var inputJson = JsonConvert.SerializeObject(json);
            HttpContent inputContent = new StringContent(inputJson, Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(BaseUrl + url, inputContent);
        }

        public async Task<string> SetAuthorization()
        {
            try
            {
                var request = new
                {
                    username = Username,
                    password = Password
                };

                var response = await Post(request, "authorize");
                var contents = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    BaseResponseModel<AuthorizeResponseData> responseModel = JsonConvert.DeserializeObject<BaseResponseModel<AuthorizeResponseData>>(contents);
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", responseModel.Data.Token);
                    return responseModel.Data.Token;
                }
                else
                {
                    BaseResponseModel<object> responseModel = JsonConvert.DeserializeObject<BaseResponseModel<object>>(contents);
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> ValidateEmail(string emailAddress)
        {
            try
            {
                await SetAuthorization();
                var request = new
                { email = emailAddress };

                var response = await Post(request, "ar/validateEmail");
                var contents = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    BaseResponseModel<object> responseModel = JsonConvert.DeserializeObject<BaseResponseModel<object>>(contents);
                    return true;
                }
                else
                {
                    BaseResponseModel<object> responseModel = JsonConvert.DeserializeObject<BaseResponseModel<object>>(contents);
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<bool> ValidateKit(string kitId)
        {
            try
            {
                await SetAuthorization();
                var request = new
                { kit_id = kitId };

                var response = await Post(request, "ar/validateKit");
                var contents = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    BaseResponseModel<object> responseModel = JsonConvert.DeserializeObject<BaseResponseModel<object>>(contents);
                    return true;
                }
                else
                {
                    BaseResponseModel<object> responseModel = JsonConvert.DeserializeObject<BaseResponseModel<object>>(contents);
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<DNAData>> GetDNAResult(string kitId)
        {
            try
            {
                await SetAuthorization();
                var request = new
                { kit_id = kitId };

                var response = await Post(request, "ar/getDNAResult");
                var contents = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    BaseResponseModel<List<List<DNAData>>> responseModel = JsonConvert.DeserializeObject<BaseResponseModel<List<List<DNAData>>>>(contents);
                    List<DNAData> result = new List<DNAData>();
                    foreach (var outerlist in responseModel.Data)
                    {
                        result.AddRange(outerlist);
                    }
                    return result;
                }
                else
                {
                    BaseResponseModel<object> responseModel = JsonConvert.DeserializeObject<BaseResponseModel<object>>(contents);
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<DNAData>> GetHealthInsightResult(string kitId, HealthInsightType healthInsightType)
        {
            try
            {
                await SetAuthorization();
                var request = new
                {
                    kit_id = kitId,
                    type = healthInsightType.ToString()
                };

                var response = await Post(request, "ar/getHealthInsightResult");
                var contents = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    BaseResponseModel<List<DNAData>> responseModel = JsonConvert.DeserializeObject<BaseResponseModel<List<DNAData>>>(contents);
                    return responseModel.Data;
                }
                else
                {
                    BaseResponseModel<object> responseModel = JsonConvert.DeserializeObject<BaseResponseModel<object>>(contents);
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<EpigeneticData>> GetEpigeneticResult(string kitId)
        {
            try
            {
                await SetAuthorization();
                var request = new
                { kit_id = kitId };
                var response = await Post(request, "ar/getEpigeneticResult");
                var contents = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    BaseResponseModel<EpigeneticResponse> responseModel = JsonConvert.DeserializeObject<BaseResponseModel<EpigeneticResponse>>(contents);
                    return responseModel.Data.EpigeneticDataList;
                }
                else
                {
                    BaseResponseModel<object> responseModel = JsonConvert.DeserializeObject<BaseResponseModel<object>>(contents);
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> SignUp(SignUpRequest signUpRequest)
        {
            try
            {
                await SetAuthorization();
                if (await ValidateEmail(signUpRequest.Email) && await ValidateKit(signUpRequest.KitId))
                {
                    //signUpRequest.Email = "ARTEST004@yopmail.com";
                    //signUpRequest.FirstName = "John";
                    //signUpRequest.LastName = "Four";
                    //signUpRequest.KitId = "ARTEST004";
                    //signUpRequest.Country = "GB";
                    //signUpRequest.Height = 164;
                    //signUpRequest.HeightUnit = "cm";
                    //signUpRequest.Weight = 64;
                    //signUpRequest.WeightUnit = "kg";
                    //signUpRequest.Gender = "male";
                    //signUpRequest.Phone = "+448989876767";
                    //signUpRequest.Birthday = "24-04-1994";

                    var response = await Post(signUpRequest, "ar/signUp");
                    var contents = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        BaseResponseModel<object> responseModel = JsonConvert.DeserializeObject<BaseResponseModel<object>>(contents);
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}