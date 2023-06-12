using Newtonsoft.Json;

namespace MuhdoApi.Net7.Model
{
    internal class BaseResponseModel<T>
    {
        [JsonProperty(PropertyName = "statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty(PropertyName = "code")]
        public int Code { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "data")]
        public T Data { get; set; }
    }
}
