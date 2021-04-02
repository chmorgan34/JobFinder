using JobFinder.Models.JSON;
using JobFinder.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JobFinder.Models
{
    public interface IApiHelper
    {
        Task<List<Job>> GetAdzunaAsync(SearchViewModel searchVM);
        Task<List<Job>> GetGithubjobsAsync(SearchViewModel searchVM);
        Task<List<Job>> GetJoobleAsync(SearchViewModel searchVM);
        Task<List<Job>> GetReedAsync(SearchViewModel searchVM);
        Task<List<Job>> GetUsajobsAsync(SearchViewModel searchVM);
    }

    public class ApiHelper : IApiHelper
    {
        private readonly HttpClient adzunaClient;
        private readonly HttpClient githubClient;
        private readonly HttpClient joobleClient;
        private readonly HttpClient reedClient;
        private readonly HttpClient usajobsClient;
        private readonly string adzunaAppID;
        private readonly string adzunaAppKey;
        private readonly string joobleApiKey;
        private readonly MediaTypeWithQualityHeaderValue jsonHeader;

        public ApiHelper(string adzunaAppID, string adzunaAppKey, string joobleApiKey, string reedApiKey, string usajobsApiKey, string usajobsUserAgent)
        {
            jsonHeader = new MediaTypeWithQualityHeaderValue("application/json");

            adzunaClient = new HttpClient();
            adzunaClient.DefaultRequestHeaders.Accept.Add(jsonHeader);
            this.adzunaAppID = adzunaAppID;
            this.adzunaAppKey = adzunaAppKey;

            githubClient = new HttpClient();
            githubClient.DefaultRequestHeaders.Accept.Add(jsonHeader);

            joobleClient = new HttpClient();
            joobleClient.DefaultRequestHeaders.Accept.Add(jsonHeader);
            this.joobleApiKey = joobleApiKey;

            reedClient = new HttpClient();
            reedClient.DefaultRequestHeaders.Accept.Add(jsonHeader);
            var base64AuthString = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{reedApiKey}:"));
            reedClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64AuthString);

            usajobsClient = new HttpClient();
            usajobsClient.DefaultRequestHeaders.Accept.Add(jsonHeader);
            usajobsClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", usajobsUserAgent);
            usajobsClient.DefaultRequestHeaders.Add("Authorization-Key", usajobsApiKey);
        }

        public async Task<List<Job>> GetAdzunaAsync(SearchViewModel searchVM)
        {
            string url = $"https://api.adzuna.com/v1/api/jobs/{searchVM.Country}/search/{searchVM.Page}" +
                $"?app_id={adzunaAppID}" +
                $"&app_key={adzunaAppKey}" +
                "&results_per_page=50" +
                $"&sort_by={searchVM.SortBy}";
            if (searchVM.Keywords != null)
                url += $"&what={searchVM.Keywords}";
            if (searchVM.Location != null)
            {
                url += $"&where={searchVM.Location}";

                if (searchVM.MilesAway != null)
                {
                    int kilometers = Convert.ToInt32((int)searchVM.MilesAway * 1.609);
                    url += $"&distance={kilometers}";
                }
            }
            if (searchVM.MinSalary != null)
                url += $"&salary_min={searchVM.MinSalary}";
            if (searchVM.FullTimeOnly)
                url += "&full_time=1";


            Stream jsonStream;
            try
            {
                jsonStream = await adzunaClient.GetStreamAsync(url);
            }
            catch (Exception e)
            {
                throw new Exception("Adzuna request failed", e);
            }

            AdzunaRoot root;
            try
            {
                root = await JsonSerializer.DeserializeAsync<AdzunaRoot>(jsonStream);
            }
            catch (Exception e)
            {
                throw new Exception("Adzuna JSON deserialization failed", e);
            }


            var results = new List<Job>();
            foreach (var adzunaJob in root.Jobs)
            {
                results.Add(new Job
                {
                    Title = Regex.Replace(adzunaJob.TitleHTML, "<.*?>", string.Empty),
                    Description = Regex.Replace(adzunaJob.DescriptionHTML, "<.*?>", string.Empty),
                    CreatedAt = adzunaJob.CreatedAt,
                    Company = adzunaJob.Company.Name,
                    Location = adzunaJob.Location.LocationString,
                    URL = adzunaJob.URL,
                    MinSalary = adzunaJob.MaxSalary,
                    MaxSalary = adzunaJob.MinSalary
                });
            }

            return results;
        }

        public async Task<List<Job>> GetGithubjobsAsync(SearchViewModel searchVM)
        {
            string url = $"https://jobs.github.com/positions.json?page={searchVM.Page}";
            if (searchVM.Keywords != null)
                url += $"&description={searchVM.Keywords}";
            if (searchVM.Location != null)
                url += $"&location={searchVM.Location}";
            if (searchVM.FullTimeOnly)
                url += "&full_time=on";


            Stream jsonStream;
            try
            {
                jsonStream = await githubClient.GetStreamAsync(url);
            }
            catch (Exception e)
            {
                throw new Exception("GitHub request failed", e);
            }

            List<GithubJob> githubJobs;
            try
            {
                githubJobs = await JsonSerializer.DeserializeAsync<List<GithubJob>>(jsonStream);
            }
            catch (Exception e)
            {
                throw new Exception("GitHub JSON deserialization failed", e);
            }


            var results = new List<Job>();
            foreach (var githubJob in githubJobs)
            {
                results.Add(new Job
                {
                    Title = githubJob.Title,
                    Description = Regex.Replace(githubJob.DescriptionHTML, "<.*?>", string.Empty),
                    CreatedAt = githubJob.GetDateTime(),
                    Company = githubJob.Company,
                    Location = githubJob.Location,
                    URL = githubJob.URL
                });
            }

            return results;
        }

        public async Task<List<Job>> GetJoobleAsync(SearchViewModel searchVM)
        {
            string url = $"https://jooble.org/api/{joobleApiKey}";
            var joobleRequest = new JoobleRequest
            {
                Keywords = searchVM.Keywords,
                Location = searchVM.Location,
                Distance = searchVM.MilesAway,
                Salary = searchVM.MinSalary,
                Page = searchVM.Page
            };

            var response = await joobleClient.PostAsJsonAsync(url, joobleRequest, JoobleRequest.SerializerOptions);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                throw new Exception("Jooble request failed", e);
            }

            var jsonStream = await response.Content.ReadAsStreamAsync();

            JoobleRoot root;
            try
            {
                root = await JsonSerializer.DeserializeAsync<JoobleRoot>(jsonStream);
            }
            catch (Exception e)
            {
                throw new Exception("Jooble deserialization failed", e);
            }

            var results = new List<Job>();
            foreach (var joobleJob in root.Jobs)
            {
                results.Add(new Job
                {
                    Title = joobleJob.Title,
                    Description = Regex.Replace(joobleJob.DescriptionHTML, "<.*?>", string.Empty).Replace("&nbsp;", string.Empty),
                    CreatedAt = joobleJob.CreatedAt,
                    Company = joobleJob.Company,
                    Location = joobleJob.Location,
                    URL = joobleJob.URL
                });
            }

            return results;
        }

        public async Task<List<Job>> GetReedAsync(SearchViewModel searchVM)
        {
            string url = $"https://www.reed.co.uk/api/1.0/search?resultsToSkip={(searchVM.Page - 1) * 100}";
            if (searchVM.Keywords != null)
                url += $"&keywords={searchVM.Keywords}";
            if (searchVM.Location != null)
            {
                url += $"&locationName={searchVM.Location}";

                if (searchVM.MilesAway != null)
                    url += $"&distanceFromLocation={searchVM.MilesAway}";
            }
            if (searchVM.FullTimeOnly)
                url += "&fullTime=true";
            if (searchVM.MinSalary != null)
                url += $"&minimumSalary={searchVM.MinSalary}";


            Stream jsonStream;
            try
            {
                jsonStream = await reedClient.GetStreamAsync(url);
            }
            catch (Exception e)
            {
                throw new Exception("Reed request failed", e);
            }

            ReedRoot root;
            try
            {
                root = await JsonSerializer.DeserializeAsync<ReedRoot>(jsonStream);
            }
            catch (Exception e)
            {
                throw new Exception("Reed JSON deserialization failed", e);
            }


            var results = new List<Job>();
            foreach (ReedJob reedJob in root.Jobs)
            {
                results.Add(new Job
                {
                    Title = reedJob.Title,
                    Description = reedJob.Description,
                    CreatedAt = reedJob.GetDateTime(),
                    Company = reedJob.Company,
                    Location = reedJob.Location,
                    URL = reedJob.URL,
                    MinSalary = (reedJob.MinSalary == null) ? null : Convert.ToInt32(reedJob.MinSalary),
                    MaxSalary = (reedJob.MaxSalary == null) ? null : Convert.ToInt32(reedJob.MaxSalary)
                });
            }

            return results;
        }

        public async Task<List<Job>> GetUsajobsAsync(SearchViewModel searchVM)
        {
            var url = $"https://data.usajobs.gov/api/Search?Page={searchVM.Page}";
            if (searchVM.Keywords != null)
                url += $"&Keyword={searchVM.Keywords}";
            if (searchVM.MinSalary != null)
                url += $"&RemunerationMinimumAmount={searchVM.MinSalary}";
            if (searchVM.Location != null)
            {
                url += $"&LocationName={searchVM.Location}";

                if (searchVM.MilesAway != null)
                    url += $"&Radius={searchVM.MilesAway}";
            }
            if (searchVM.FullTimeOnly)
                url += "&PositionSchedule=1";
            if (searchVM.SortBy == "date")
                url += "&SortField=OpenDate&SortDirection=Desc";
            else if (searchVM.SortBy == "salary")
                url += "&SortField=Salary&SortDirection=Desc";


            Stream jsonStream;
            try
            {
                jsonStream = await usajobsClient.GetStreamAsync(url);
            }
            catch (Exception e)
            {
                throw new Exception("USAJOBS request failed", e);
            }

            USAJobsRoot root;
            try
            {
                root = await JsonSerializer.DeserializeAsync<USAJobsRoot>(jsonStream);
            }
            catch (Exception e)
            {
                throw new Exception("USAJOBS JSON deserialization failed", e);
            }


            var results = new List<Job>();
            foreach (var usajobsJob in root.SearchResult.Jobs)
            {
                results.Add(new Job
                {
                    Title = usajobsJob.Details.Title,
                    Description = usajobsJob.Details.UserArea.Details.Description,
                    CreatedAt = usajobsJob.Details.CreatedAt,
                    Company = usajobsJob.Details.Company,
                    Location = usajobsJob.Details.Location,
                    URL = usajobsJob.Details.URL,
                    MinSalary = Convert.ToInt32(Convert.ToDouble(usajobsJob.Details.SalaryRange[0].MinSalary)),
                    MaxSalary = Convert.ToInt32(Convert.ToDouble(usajobsJob.Details.SalaryRange[0].MaxSalary))
                });
            }

            return results;
        }
    }
}
