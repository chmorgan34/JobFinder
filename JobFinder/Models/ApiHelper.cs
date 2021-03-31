using JobFinder.Models.Deserializers;
using JobFinder.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JobFinder.Models
{
    public interface IApiHelper
    {
        Task<List<Job>> GetAdzuna(SearchViewModel searchVM);
        Task<List<Job>> GetGithub(SearchViewModel searchVM);
        Task<List<Job>> GetReed(SearchViewModel searchVM);
    }

    public class ApiHelper : IApiHelper
    {
        private readonly HttpClient client;
        private readonly string adzunaAppID;
        private readonly string adzunaAppKey;
        private readonly string reedApiKey;

        public ApiHelper(string adzunaAppID, string adzunaAppKey, string reedApiKey)
        {
            client = new HttpClient();
            this.adzunaAppID = adzunaAppID;
            this.adzunaAppKey = adzunaAppKey;
            this.reedApiKey = reedApiKey;
        }

        public async Task<List<Job>> GetAdzuna(SearchViewModel searchVM)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string url = $"https://api.adzuna.com/v1/api/jobs/{searchVM.Country}/search/{searchVM.Page}" +
                $"?app_id={adzunaAppID}" +
                $"&app_key={adzunaAppKey}" +
                "&results_per_page=50" +
                $"&sort_by={searchVM.SortBy}";
            if (searchVM.Keywords != null)
                url += $"&what={searchVM.Keywords}";
            if (searchVM.Location != null)
                url += $"&where={searchVM.Location}";
            if (searchVM.MilesAway != null)
            {
                int kilometers = Convert.ToInt32((int)searchVM.MilesAway * 1.609);
                url += $"&distance={kilometers}";
            }
            if (searchVM.MinSalary != null)
                url += $"&salary_min={searchVM.MinSalary}";
            if (searchVM.FullTimeOnly)
                url += "&full_time=1";

            Stream jsonStream;
            try
            {
                jsonStream = await client.GetStreamAsync(url);
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

        public async Task<List<Job>> GetGithub(SearchViewModel searchVM)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

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
                jsonStream = await client.GetStreamAsync(url);
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

        public async Task<List<Job>> GetReed(SearchViewModel searchVM)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var base64AuthString = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{reedApiKey}:"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64AuthString);

            string url = $"https://www.reed.co.uk/api/1.0/search?resultsToSkip={(searchVM.Page - 1) * 100}";
            if (searchVM.Keywords != null)
                url += $"&keywords={searchVM.Keywords}";
            if (searchVM.Location != null)
                url += $"&locationName={searchVM.Location}";
            if (searchVM.MilesAway != null)
                url += $"&distanceFromLocation={searchVM.MilesAway}";
            if (searchVM.FullTimeOnly)
                url += "&fullTime=true";
            if (searchVM.MinSalary != null)
                url += $"&minimumSalary={searchVM.MinSalary}";

            Stream jsonStream;
            try
            {
                jsonStream = await client.GetStreamAsync(url);
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
    }
}
