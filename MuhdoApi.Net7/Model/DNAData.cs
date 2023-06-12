using Newtonsoft.Json;

namespace MuhdoApi.Net7.Model
{
    public class DNAData
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "genes_of_interest")]
        public string GenesOfInterest { get; set; }

        [JsonProperty(PropertyName = "intro")]
        public string Intro { get; set; }

        [JsonProperty(PropertyName = "output")]
        public OutPutData Output { get; set; }

        [JsonProperty(PropertyName = "references")]
        public ReferenceData[] ReferenceList { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
    }

    public class OutPutData
    {
        [JsonProperty(PropertyName = "indicator")]
        public int Indicator { get; set; }

        [JsonProperty(PropertyName = "result")]
        public string Result { get; set; }

        [JsonProperty(PropertyName = "recommondation")]
        public string Recommendation { get; set; }
    }

}
