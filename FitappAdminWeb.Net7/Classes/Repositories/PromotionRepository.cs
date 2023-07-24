using Azure.Core;
using ChartApi.Net7;
using FitappAdminWeb.Net7.Classes.Utilities;
using FitappAdminWeb.Net7.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace FitappAdminWeb.Net7.Classes.Repositories
{
    public class PromotionRepository
    {
        private IConfiguration _configuration;
        private ILogger<PromotionRepository> _logger;
        private BlobStorageRepository _blobrepo;
        private FitAppAPIUtil _apiutil;

        private readonly string URL_SENDPROMOTIONCHAT = "/api/Chat/SendPromotionChat";
        private readonly string APPSETTING_MAINAPIDOMAIN = "MainApi_Domain";

        public PromotionRepository( 
            IConfiguration configuration, 
            ILogger<PromotionRepository> logger, 
            BlobStorageRepository blobrepo,
            FitAppAPIUtil apiutil)
        {
            _configuration = configuration;
            _logger = logger;
            _blobrepo = blobrepo;
            _apiutil = apiutil;
        }

        public async Task<bool> SendPromotionChatMessage(PromotionModel data)
        {
            using (_logger.BeginScope("SendPromotionChatMessage"))
            {
                try
                {
                    var apidomain = _configuration.GetValue<string>(APPSETTING_MAINAPIDOMAIN);
                    if (String.IsNullOrEmpty(apidomain))
                    {
                        _logger.LogError("API Domain not found in app setting {setting}.", APPSETTING_MAINAPIDOMAIN);
                        return false;
                    }

                    //build promotion message dto
                    PromotionChatmessage message = new PromotionChatmessage();

                    message.Icon = data.Icon;
                    message.Title = data.Title;
                    message.Message = data.Message ?? null;
                    message.LinkUrl = data.Url ?? null;
                    message.DateSentUTC = DateTime.UtcNow;
                    message.ImageUrl = null;

                    //upload image to blob and retrieve blob url
                    if (data.ImageBytes != null && data.ImageBytes.Length > 0 && data.ContentType != null)
                    {
                        var imageUrl = await _blobrepo.UploadImageUrl(data.ImageBytes, data.ContentType);
                        if (BlobStorageRepository.FILEUPLOAD_ERRORSTRING.Equals(imageUrl, StringComparison.OrdinalIgnoreCase))
                        {
                            _logger.LogError("Failed to upload image to blob");
                            return false;
                        }
                        message.ImageUrl = imageUrl;
                    }

                    //send promotion message dto to API as POST
                    //var url = $"{apidomain}{URL_SENDPROMOTIONCHAT}";
                    //var httpclient = _httpclientfactory.CreateClient();

                    //var request = new HttpRequestMessage(HttpMethod.Post, url);
                    //var stringcontent = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");
                    //request.Content = stringcontent;
                    var httpclient = _apiutil.GetHttpClient();
                    var request = _apiutil.BuildRequest(URL_SENDPROMOTIONCHAT, HttpMethod.Post, message);

                    var response = await httpclient.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    //read boolean response
                    string responseString = await response.Content.ReadAsStringAsync();
                    return responseString.Equals(Boolean.TrueString, StringComparison.OrdinalIgnoreCase);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send promotion message");
                    return false;
                }
            }
        }
    }
}
