using Newtonsoft.Json;

namespace MuhdoApi.Net7.Model
{
    public class SignUpRequest
    {
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "kit_id")]
        public string KitId { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }

        [JsonProperty(PropertyName = "height")]
        public int Height { get; set; }

        [JsonProperty(PropertyName = "height_unit")]
        public string HeightUnit { get; set; }

        [JsonProperty(PropertyName = "weight")]
        public int Weight { get; set; }

        [JsonProperty(PropertyName = "weight_unit")]
        public string WeightUnit { get; set; }

        [JsonProperty(PropertyName = "gender")]
        public string Gender { get; set; }

        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }

        [JsonProperty(PropertyName = "birthday")]
        public string Birthday { get; set; }
    }
}
