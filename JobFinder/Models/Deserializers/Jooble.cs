using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JobFinder.Models.Deserializers
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
        public string Description { get; set; }

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
}
