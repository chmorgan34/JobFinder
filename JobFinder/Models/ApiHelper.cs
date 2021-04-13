using JobFinder.Models;
using JobFinder.Models.JSON;
using JobFinder.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        Task<List<Job>> GetThemuseAsync(SearchViewModel searchVM);
        Task<List<Job>> GetUsajobsAsync(SearchViewModel searchVM);
    }

    public class ApiHelper : IApiHelper
    {
        private readonly HttpClient adzunaClient;
        private readonly HttpClient githubClient;
        private readonly HttpClient joobleClient;
        private readonly HttpClient reedClient;
        private readonly HttpClient themuseClient;
        private readonly HttpClient usajobsClient;
        private readonly string adzunaAppID;
        private readonly string adzunaAppKey;
        private readonly string joobleApiKey;
        private readonly string themuseApiKey;

        public ApiHelper(string adzunaAppID, string adzunaAppKey, string joobleApiKey, string reedApiKey, string themuseApiKey,
            string usajobsApiKey, string usajobsUserAgent)
        {
            var jsonHeader = new MediaTypeWithQualityHeaderValue("application/json");

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

            themuseClient = new HttpClient();
            themuseClient.DefaultRequestHeaders.Accept.Add(jsonHeader);
            this.themuseApiKey = themuseApiKey;

            usajobsClient = new HttpClient();
            usajobsClient.DefaultRequestHeaders.Accept.Add(jsonHeader);
            usajobsClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", usajobsUserAgent);
            usajobsClient.DefaultRequestHeaders.Add("Authorization-Key", usajobsApiKey);
        }

        // https://developer.adzuna.com/activedocs#!/adzuna/search
        public async Task<List<Job>> GetAdzunaAsync(SearchViewModel searchVM)
        {
            var url = $"https://api.adzuna.com/v1/api/jobs/{searchVM.AdzunaCountry}/search/{searchVM.Page}" +
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


            var jsonStream = await adzunaClient.GetStreamAsync(url);
            var root = await JsonSerializer.DeserializeAsync<AdzunaRoot>(jsonStream);

            string cultureName;
            switch (searchVM.AdzunaCountry)
            {
                case "us":
                    cultureName = "en-US";
                    break;
                case "at":
                    cultureName = "en-AT";
                    break;
                case "br":
                    cultureName = "pt-BR";
                    break;
                case "ca":
                    cultureName = "en-CA";
                    break;
                case "de":
                    cultureName = "en-DE";
                    break;
                case "fr":
                    cultureName = "fr-FR";
                    break;
                case "in":
                    cultureName = "en-IN";
                    break;
                case "it":
                    cultureName = "it-IT";
                    break;
                case "nl":
                    cultureName = "en-NL";
                    break;
                case "nz":
                    cultureName = "en-NZ";
                    break;
                case "pl":
                    cultureName = "pl-PL";
                    break;
                case "ru":
                    cultureName = "ru-RU";
                    break;
                case "sg":
                    cultureName = "en-SG";
                    break;
                case "za":
                    cultureName = "en-ZA";
                    break;
                case "uk":
                    cultureName = "en-GB";
                    break;
                default:
                    throw new Exception("Adzuna country does not exist.");
            }
            var salaryCulture = new CultureInfo(cultureName, false);
            salaryCulture.NumberFormat.CurrencyDecimalDigits = 0;

            var results = new List<Job>();
            foreach (var adzunaJob in root.Jobs)
            {
                var job = new Job
                {
                    Title = adzunaJob.TitleHTML,
                    Description = adzunaJob.DescriptionHTML,
                    CreatedAt = adzunaJob.CreatedAt,
                    Company = adzunaJob.Company.Name,
                    Location = adzunaJob.Location.LocationString,
                    URL = adzunaJob.URL,
                    MinSalary = adzunaJob.MinSalary,
                    MaxSalary = adzunaJob.MaxSalary,
                    SalaryCulture = salaryCulture
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
                        job.EmploymentType = EmploymentLength.Permanent;
                        break;
                    case "contract":
                        job.EmploymentType = EmploymentLength.Temporary;
                        break;
                    default:
                        break;
                }

                results.Add(job);
            }

            return results;
        }

        // https://jobs.github.com/api
        public async Task<List<Job>> GetGithubjobsAsync(SearchViewModel searchVM)
        {
            var url = $"https://jobs.github.com/positions.json?page={searchVM.Page}";
            if (searchVM.Keywords != null)
                url += $"&description={searchVM.Keywords}";
            if (searchVM.Location != null)
                url += $"&location={searchVM.Location}";
            if (searchVM.FullTimeOnly)
                url += "&full_time=on";


            var jsonStream = await githubClient.GetStreamAsync(url);
            var githubJobs = await JsonSerializer.DeserializeAsync<List<GithubJob>>(jsonStream);

            var results = new List<Job>();
            foreach (var githubJob in githubJobs)
            {
                var job = new Job
                {
                    Title = githubJob.Title,
                    Description = githubJob.DescriptionHTML,
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

        // https://jooble.org/api/about
        public async Task<List<Job>> GetJoobleAsync(SearchViewModel searchVM)
        {
            var url = $"https://jooble.org/api/{joobleApiKey}";
            var joobleRequest = new JoobleRequest
            {
                Keywords = searchVM.Keywords,
                Location = searchVM.Location,
                Distance = searchVM.MilesAway,
                Salary = searchVM.MinSalary,
                Page = searchVM.Page
            };

            var response = await joobleClient.PostAsJsonAsync(url, joobleRequest, JoobleRequest.SerializerOptions);
            response.EnsureSuccessStatusCode();

            var jsonStream = await response.Content.ReadAsStreamAsync();
            var root = await JsonSerializer.DeserializeAsync<JoobleRoot>(jsonStream);


            var salaryCulture = new CultureInfo("en-US", false);
            salaryCulture.NumberFormat.CurrencyDecimalDigits = 0;

            var results = new List<Job>();
            foreach (var joobleJob in root.Jobs)
            {
                if (searchVM.FullTimeOnly && joobleJob.JobType == "Part-time")
                    continue;
                if (searchVM.PermanentOnly && (joobleJob.JobType == "Temporary" || joobleJob.JobType == "Internship"))
                    continue;

                var job = new Job
                {
                    Title = joobleJob.Title,
                    Description = joobleJob.DescriptionHTML,
                    CreatedAt = joobleJob.CreatedAt,
                    Company = joobleJob.Company,
                    Location = joobleJob.Location,
                    URL = joobleJob.URL,
                    SalaryCulture = salaryCulture
                };


                var salaryStr = joobleJob.SalaryString;
                var isHourly = salaryStr.Contains("per hour");
                salaryStr = salaryStr.Replace("per hour", string.Empty);
                salaryStr = salaryStr.Replace("k", string.Empty);
                salaryStr = salaryStr.Replace("$", string.Empty).Trim();

                var salaryRange = salaryStr.Split('-', StringSplitOptions.TrimEntries);
                switch (salaryRange.Length)
                {
                    case 1:
                        if (salaryRange[0] != string.Empty)
                            job.MinSalary = isHourly ? Convert.ToInt32(Convert.ToDouble(salaryRange[0])) : Convert.ToInt32(salaryRange[0]) * 1000;
                        break;
                    case 2:
                        if (salaryRange[0] != string.Empty)
                            job.MinSalary = isHourly ? Convert.ToInt32(Convert.ToDouble(salaryRange[0])) : Convert.ToInt32(salaryRange[0]) * 1000;
                        if (salaryRange[1] != string.Empty)
                            job.MaxSalary = isHourly ? Convert.ToInt32(Convert.ToDouble(salaryRange[1])) : Convert.ToInt32(salaryRange[1]) * 1000;
                        break;
                    default:
                        break;
                }


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
                        job.EmploymentType = EmploymentLength.Temporary;
                        break;
                    default:
                        break;
                }

                results.Add(job);
            }

            return results;
        }

        // https://www.reed.co.uk/developers/jobseeker
        public async Task<List<Job>> GetReedAsync(SearchViewModel searchVM)
        {
            var url = $"https://www.reed.co.uk/api/1.0/search?resultsToSkip={(searchVM.Page - 1) * 100}";
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


            var jsonStream = await reedClient.GetStreamAsync(url);
            var root = await JsonSerializer.DeserializeAsync<ReedRoot>(jsonStream);


            CultureInfo salaryCulture = new CultureInfo("en-GB", false);
            salaryCulture.NumberFormat.CurrencyDecimalDigits = 0;

            var results = new List<Job>();
            foreach (var reedJob in root.Jobs)
            {
                var job = new Job
                {
                    Title = reedJob.Title,
                    Description = reedJob.Description,
                    CreatedAt = reedJob.GetDateTime(),
                    Company = reedJob.Company,
                    Location = reedJob.Location,
                    URL = reedJob.URL,
                    MinSalary = (reedJob.MinSalary == null) ? null : Convert.ToInt32(reedJob.MinSalary),
                    MaxSalary = (reedJob.MaxSalary == null) ? null : Convert.ToInt32(reedJob.MaxSalary),
                    SalaryCulture = salaryCulture
                };
            }

            return results;
        }

        // https://www.themuse.com/developers/api/v2
        public async Task<List<Job>> GetThemuseAsync(SearchViewModel searchVM)
        {
            var url = $"https://www.themuse.com/api/public/jobs?api_key={themuseApiKey}&page={searchVM.Page - 1}" +
                $"&category={searchVM.ThemuseCategory}";
            if (searchVM.Location != null)
                url += $"&location={searchVM.Location}";


            var jsonStream = await themuseClient.GetStreamAsync(url);
            var root = await JsonSerializer.DeserializeAsync<TheMuseRoot>(jsonStream);

            var results = new List<Job>();
            foreach (var themuseJob in root.Results)
            {
                var job = new Job
                {
                    Title = themuseJob.Title,
                    Description = themuseJob.DescriptionHTML,
                    CreatedAt = themuseJob.CreatedAt,
                    Company = themuseJob.Company.Name,
                    URL = themuseJob.Refs.URL
                };

                if (themuseJob.Locations.Count > 0)
                    job.Location = themuseJob.Locations[0].Location;

                results.Add(job);
            }

            return results;
        }

        // https://developer.usajobs.gov/API-Reference/GET-api-Search
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
                url += "&SortField=OpenDate";
            else if (searchVM.SortBy == "salary")
                url += "&SortField=Salary";
            if (searchVM.SortDirection == "up")
                url += "&SortDirection=Asc";
            else if (searchVM.SortDirection == "down")
                url += "&SortDirection=Desc";

            var jsonStream = await usajobsClient.GetStreamAsync(url);
            var root = await JsonSerializer.DeserializeAsync<USAJobsRoot>(jsonStream);


            var salaryCulture = new CultureInfo("en-US", false);
            salaryCulture.NumberFormat.CurrencyDecimalDigits = 0;

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
                    MaxSalary = Convert.ToInt32(Convert.ToDouble(usajobsJob.Details.SalaryRange[0].MaxSalary)),
                    SalaryCulture = salaryCulture
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
                        job.EmploymentType = EmploymentLength.Permanent;
                        break;
                    default:
                        job.EmploymentType = EmploymentLength.Temporary;
                        break;
                }

                results.Add(job);
            }

            return results;
        }
    }
}
