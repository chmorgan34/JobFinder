using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JobFinder.Models.JSON
{
    public class GithubJob
    {
        [JsonPropertyName("type")]
        public string JobType { get; set; }

        [JsonPropertyName("url")]
        public string URL { get; set; }

        [JsonPropertyName("created_at")]
        public string DateTimeString { get; set; }

        [JsonPropertyName("company")]
        public string Company { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string DescriptionHTML { get; set; }


        [JsonIgnore]
        private readonly string dateTimeFormat = "MMM dd HH:mm:ss yyyy";

        public DateTime GetDateTime()
        {
            var fixedDateString = DateTimeString.Remove(0, 4).Replace("UTC ", "");

            return DateTime.ParseExact(fixedDateString, dateTimeFormat, System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
