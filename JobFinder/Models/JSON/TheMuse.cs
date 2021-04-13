using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JobFinder.Models.JSON
{
    public class TheMuseRoot
    {
        [JsonPropertyName("results")]
        public List<TheMuseJob> Results { get; set; }
    }

    public class TheMuseJob
    {
        [JsonPropertyName("contents")]
        public string DescriptionHTML { get; set; }

        [JsonPropertyName("name")]
        public string Title { get; set; }

        [JsonPropertyName("publication_date")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("locations")]
        public List<TheMuseLocation> Locations { get; set; }

        [JsonPropertyName("refs")]
        public TheMuseRefs Refs { get; set; }

        [JsonPropertyName("company")]
        public TheMuseCompany Company { get; set; }
    }

    public class TheMuseLocation
    {
        [JsonPropertyName("name")]
        public string Location { get; set; }
    }

    public class TheMuseCompany
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class TheMuseRefs
    {
        [JsonPropertyName("landing_page")]
        public string URL { get; set; }
    }
}
