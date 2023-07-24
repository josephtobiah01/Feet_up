using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;

namespace FitappAdminWeb.Net7.Classes.Utilities
{
    public class FitAppAPIUtil
    {
        private readonly string APPSETTING_DOMAIN = "MainApi_Domain";
        private readonly string APPSETTING_APIKEY = "MainApi_ApiKey";

        string apikey_value = String.Empty;
        string apidomain_value = String.Empty;

        private IHttpClientFactory _httpClientFactory;
        private IConfiguration _configuration;
        public FitAppAPIUtil(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            apikey_value = _configuration.GetValue<string>(APPSETTING_APIKEY) ?? String.Empty;
            apidomain_value = _configuration.GetValue<string>(APPSETTING_DOMAIN) ?? String.Empty;
        }

        public HttpClient GetHttpClient()
        {
            return _httpClientFactory.CreateClient();
        }

        /// <summary>
        /// Returns a HttpRequestMessage instance for use in FitApp API.
        /// This method automatically uses the configuration values from AppSettings.
        /// </summary>
        /// <param name="url">URL to be used. Domain is already added to this based on settings.</param>
        /// <param name="method">HTTPMethod for the request</param>
        /// <param name="content">Content to be placed in the request body. This will always be converted to JSON, UTF-8.</param>
        /// <returns>HttpRequestMessage instance to be passed to HttpClient</returns>
        public HttpRequestMessage BuildRequest(string url, HttpMethod method, object content = null)
        {
            string finalUrl = apidomain_value + url;
            HttpRequestMessage request = new HttpRequestMessage(method, finalUrl);

            //add security headers if available
            if (!String.IsNullOrEmpty(apikey_value))
            {
                request.Headers.Add("Authorization", apikey_value);
            }

            //add content (JSON) if available
            if (content != null)
            {
                request.Content = JsonContent.Create(content);
            }
            return request;
        }
    }
}
