using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JobFinder.Models.JSON
{
    public class JoobleRoot
    {
        [JsonPropertyName("totalCount")]
        public int ResultCount { get; set; }

        [JsonPropertyName("jobs")]
        public List<JoobleJob> Jobs { get; set; }
    }

    public class JoobleJob
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("snippet")]
        public string DescriptionHTML { get; set; }

        [JsonPropertyName("salary")]
        public string SalaryString { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("link")]
        public string URL { get; set; }

        [JsonPropertyName("company")]
        public string Company { get; set; }

        [JsonPropertyName("updated")]
        public DateTime CreatedAt { get; set; }
    }

    public class JoobleRequest
    {
        [JsonPropertyName("keywords")]
        public string Keywords { get; set; } = "";

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("radius")]
        public int? Distance { get; set; }

        [JsonPropertyName("salary")]
        public int? Salary { get; set; }

        [JsonPropertyName("page")]
        public int? Page { get; set; }

        [JsonIgnore]
        public static JsonSerializerOptions SerializerOptions { get; } = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }
}
