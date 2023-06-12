using Newtonsoft.Json;

namespace MuhdoApi.Net7.Model
{
    internal class AuthorizeResponseData
    {
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
    }
}
