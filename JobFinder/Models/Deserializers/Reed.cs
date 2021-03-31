using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JobFinder.Models.Deserializers
{
    public class ReedRoot
    {
        [JsonPropertyName("results")]
        public List<ReedJob> Jobs { get; set; }
    }

    public class ReedJob
    {
        [JsonPropertyName("employerName")]
        public string Company { get; set; }

        [JsonPropertyName("jobTitle")]
        public string Title { get; set; }

        [JsonPropertyName("locationName")]
        public string Location { get; set; }

        [JsonPropertyName("minimumSalary")]
        public double? MinSalary { get; set; }

        [JsonPropertyName("maximumSalary")]
        public double? MaxSalary { get; set; }

        [JsonPropertyName("date")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("jobDescription")]
        public string Description { get; set; }

        [JsonPropertyName("jobUrl")]
        public string URL { get; set; }


        [JsonIgnore]
        private readonly string dateFormat = "dd/MM/yyyy";

        public DateTime GetDateTime()
        {
            return DateTime.ParseExact(CreatedAt, dateFormat, System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
