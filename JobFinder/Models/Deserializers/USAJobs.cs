using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JobFinder.Models.Deserializers
{
    public class USAJobsRoot
    {
        [JsonPropertyName("SearchResult")]
        public USAJobsSearchResult SearchResult { get; set; }
    }

    public class USAJobsSearchResult
    {
        [JsonPropertyName("SearchResultItems")]
        public List<USAJobsJob> Jobs { get; set; }
    }

    public class USAJobsJob
    {
        [JsonPropertyName("MatchedObjectDescriptor")]
        public USAJobsJobDetails Details { get; set; }
    }

    public class USAJobsJobDetails
    {
        [JsonPropertyName("PositionTitle")]
        public string Title { get; set; }

        [JsonPropertyName("PositionURI")]
        public string URL { get; set; }

        [JsonPropertyName("PositionLocationDisplay")]
        public string Location { get; set; }

        [JsonPropertyName("OrganizationName")]
        public string Company { get; set; }

        [JsonPropertyName("PositionRemuneration")]
        public List<USAJobsSalaryRange> SalaryRange { get; set; }

        [JsonPropertyName("PublicationStartDate")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("UserArea")]
        public USAJobsUserArea UserArea { get; set; }
    }

    public class USAJobsSalaryRange
    {
        [JsonPropertyName("MinimumRange")]
        public string MinSalary { get; set; }

        [JsonPropertyName("MaximumRange")]
        public string MaxSalary { get; set; }
    }

    public class USAJobsUserArea
    {
        [JsonPropertyName("Details")]
        public USAJobsUserAreaDetails Details { get; set; }
    }

    public class USAJobsUserAreaDetails
    {
        [JsonPropertyName("JobSummary")]
        public string Description { get; set; }
    }
}
