using JobFinder.Models;
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
            if (searchVM.PermanentOnly)
                url += "&permanent=1";


            Stream jsonStream;
            try
            {
                jsonStream = await adzunaClient.GetStreamAsync(url);
            }
            catch (Exception e)
            {
                throw new Exception("Adzuna request failed", e);
            }

            var root = await JsonSerializer.DeserializeAsync<AdzunaRoot>(jsonStream);

            var results = new List<Job>();
            foreach (var adzunaJob in root.Jobs)
            {
                var job = new Job
                {
                    Title = Regex.Replace(adzunaJob.TitleHTML, "<.*?>", string.Empty),
                    Description = Regex.Replace(adzunaJob.DescriptionHTML, "<.*?>", string.Empty),
                    CreatedAt = adzunaJob.CreatedAt,
                    Company = adzunaJob.Company.Name,
                    Location = adzunaJob.Location.LocationString,
                    URL = adzunaJob.URL,
                    MinSalary = adzunaJob.MaxSalary,
                    MaxSalary = adzunaJob.MinSalary
                };

                switch (adzunaJob.ContractTime)
                {
                    case "full_time":
                        job.Schedule = JobSchedule.FullTime;
                        break;
                    case "part_time":
                        job.Schedule = JobSchedule.PartTime;
                        break;
                    default:
                        break;
                }
                switch (adzunaJob.ContractType)
                {
                    case "permanent":
                        job.EmploymentType = EmploymentType.Permanent;
                        break;
                    case "contract":
                        job.EmploymentType = EmploymentType.Temporary;
                        break;
                    default:
                        break;
                }

                results.Add(job);
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

            var githubJobs = await JsonSerializer.DeserializeAsync<List<GithubJob>>(jsonStream);

            var results = new List<Job>();
            foreach (var githubJob in githubJobs)
            {
                var job = new Job
                {
                    Title = githubJob.Title,
                    Description = Regex.Replace(githubJob.DescriptionHTML, "<.*?>", string.Empty),
                    CreatedAt = githubJob.GetDateTime(),
                    Company = githubJob.Company,
                    Location = githubJob.Location,
                    URL = githubJob.URL
                };

                switch (githubJob.JobType)
                {
                    case "Full Time":
                        job.Schedule = JobSchedule.FullTime;
                        break;
                    case "Part Time":
                        job.Schedule = JobSchedule.PartTime;
                        break;
                    default:
                        break;
                }

                results.Add(job);
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
            var root = await JsonSerializer.DeserializeAsync<JoobleRoot>(jsonStream);

            var results = new List<Job>();
            foreach (var joobleJob in root.Jobs)
            {
                if (searchVM.FullTimeOnly && joobleJob.JobType != "Full-time")
                    continue;
                if (searchVM.PermanentOnly)
                {
                    if (joobleJob.JobType == "Temporary" || joobleJob.JobType == "Internship")
                        continue;
                }

                var job = new Job
                {
                    Title = joobleJob.Title,
                    Description = Regex.Replace(joobleJob.DescriptionHTML, "<.*?>", string.Empty).Replace("&nbsp;", string.Empty),
                    CreatedAt = joobleJob.CreatedAt,
                    Company = joobleJob.Company,
                    Location = joobleJob.Location,
                    URL = joobleJob.URL
                };

                switch (joobleJob.JobType)
                {
                    case "Full-time":
                        job.Schedule = JobSchedule.FullTime;
                        break;
                    case "Part-time":
                        job.Schedule = JobSchedule.PartTime;
                        break;
                    case "Temporary":
                    case "Internship":
                        job.EmploymentType = EmploymentType.Temporary;
                        break;
                    default:
                        break;
                }

                results.Add(job);
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
            if (searchVM.PermanentOnly)
                url += "&permanent=true";
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

            var root = await JsonSerializer.DeserializeAsync<ReedRoot>(jsonStream);

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
            if (searchVM.PermanentOnly)
                url += "&PositionOfferingTypeCode=15317";
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

            var root = await JsonSerializer.DeserializeAsync<USAJobsRoot>(jsonStream);

            var results = new List<Job>();
            foreach (var usajobsJob in root.SearchResult.Jobs)
            {
                var job = new Job
                {
                    Title = usajobsJob.Details.Title,
                    Description = usajobsJob.Details.UserArea.Details.Description,
                    CreatedAt = usajobsJob.Details.CreatedAt,
                    Company = usajobsJob.Details.Company,
                    Location = usajobsJob.Details.Location,
                    URL = usajobsJob.Details.URL,
                    MinSalary = Convert.ToInt32(Convert.ToDouble(usajobsJob.Details.SalaryRange[0].MinSalary)),
                    MaxSalary = Convert.ToInt32(Convert.ToDouble(usajobsJob.Details.SalaryRange[0].MaxSalary))
                };

                switch (usajobsJob.Details.PositionSchedule[0].Code)
                {
                    case "1":
                        job.Schedule = JobSchedule.FullTime;
                        break;
                    case "2":
                        job.Schedule = JobSchedule.PartTime;
                        break;
                    default:
                        break;
                }
                switch (usajobsJob.Details.OfferingType[0].Code)
                {
                    case "15317":
                        job.EmploymentType = EmploymentType.Permanent;
                        break;
                    default:
                        job.EmploymentType = EmploymentType.Temporary;
                        break;
                }

                results.Add(job);
            }

            return results;
        }
    }
}
