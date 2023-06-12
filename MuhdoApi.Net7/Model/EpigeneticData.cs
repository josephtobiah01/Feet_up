using Newtonsoft.Json;

namespace MuhdoApi.Net7.Model
{
    public class EpigeneticResponse
    {
        [JsonProperty(PropertyName = "epigenetic_data")]
        public List<EpigeneticData> EpigeneticDataList { get; set; }
    }
    public class EpigeneticData
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "Data")]
        public EpigeneticInfo EpigeneticInfo { get; set; }
    }

    public class EpigeneticInfo
    {
        [JsonProperty(PropertyName = "info")]
        public string Info { get; set; }

        [JsonProperty(PropertyName = "introduction")]
        public string Introduction { get; set; }

        [JsonProperty(PropertyName = "outcome")]
        public string Outcome { get; set; }

        [JsonProperty(PropertyName = "recommendations")]
        public string Recommendation { get; set; }

        [JsonProperty(PropertyName = "img")]
        public string Image { get; set; }

        [JsonProperty(PropertyName = "recommendations_info")]
        public List<RecommendationsInfo> RecommendationsInfoList { get; set; }

        [JsonProperty(PropertyName = "references")]
        public ReferenceData[] ReferenceList { get; set; }

        [JsonProperty(PropertyName = "score_data")]
        public ScoreData ScoreData { get; set; }
    }

    public class RecommendationsInfo
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "img")]
        public string Image { get; set; }

        [JsonProperty(PropertyName = "selected_img")]
        public string SelectedImage { get; set; }

        [JsonProperty(PropertyName = "image")]
        public string ImageUrl { get; set; }
    }

    public class ScoreData
    {
        [JsonProperty(PropertyName = "score")]
        public double Score { get; set; }

        [JsonProperty(PropertyName = "color")]
        public string Color { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }
    }

}
