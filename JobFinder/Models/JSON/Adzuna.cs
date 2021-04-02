using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JobFinder.Models.JSON
{
    public class AdzunaRoot
    {
        [JsonPropertyName("results")]
        public List<AdzunaJob> Jobs { get; set; }
    }

    public class AdzunaJob
    {
        [JsonPropertyName("company")]
        public AdzunaCompany Company { get; set; }

        [JsonPropertyName("created")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("description")]
        public string DescriptionHTML { get; set; }

        [JsonPropertyName("location")]
        public AdzunaLocation Location { get; set; }

        [JsonPropertyName("redirect_url")]
        public string URL { get; set; }

        [JsonPropertyName("salary_max")]
        public int? MinSalary { get; set; }

        [JsonPropertyName("salary_min")]
        public int? MaxSalary { get; set; }

        [JsonPropertyName("title")]
        public string TitleHTML { get; set; }
    }

    public class AdzunaCompany
    {
        [JsonPropertyName("display_name")]
        public string Name { get; set; }
    }

    public class AdzunaLocation
    {
        [JsonPropertyName("display_name")]
        public string LocationString { get; set; }
    }
}
