using Newtonsoft.Json;

namespace MuhdoApi.Net7.Model
{
    public class ReferenceData
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "links")]
        public string[] Links { get; set; }
    }
}
