﻿using JobFinder.Models.Deserializers;
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
        Task<List<Job>> GetAdzuna(SearchViewModel viewModel);
        Task<List<Job>> GetGithub(SearchViewModel viewModel);
    }

    public class ApiHelper : IApiHelper
    {
        private readonly HttpClient client;
        private readonly string adzunaAppID;
        private readonly string adzunaAppKey;

        public ApiHelper(string adzunaAppID, string adzunaAppKey)
        {
            client = new HttpClient();
            this.adzunaAppID = adzunaAppID;
            this.adzunaAppKey = adzunaAppKey;
        }

        public async Task<List<Job>> GetAdzuna(SearchViewModel viewModel)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var url = $"https://api.adzuna.com/v1/api/jobs/{viewModel.Country}/search/{viewModel.Page}" +
                $"?app_id={adzunaAppID}" +
                $"&app_key={adzunaAppKey}" +
                "&results_per_page=50" +
                $"&sort_by={viewModel.SortBy}";
            if (viewModel.Description != null)
                url += $"&what={viewModel.Description}";
            if (viewModel.Location != null)
                url += $"&where={viewModel.Location}";
            if (viewModel.Distance != null)
                url += $"&distance={viewModel.Distance}";
            if (viewModel.MinSalary != null)
                url += $"&salary_min={viewModel.MinSalary}";
            if (viewModel.FullTimeOnly)
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
            foreach (var job in root.Jobs)
            {
                results.Add(new Job
                {
                    Title = Regex.Replace(job.TitleHTML, "<.*?>", string.Empty),
                    Description = Regex.Replace(job.DescriptionHTML, "<.*?>", string.Empty),
                    CreatedAt = job.CreatedAt,
                    Company = job.Company.Name,
                    Location = job.Location.LocationString,
                    URL = job.URL,
                    SalaryMin = job.SalaryMin,
                    SalaryMax = job.SalaryMax
                });
            }

            return results;
        }

        public async Task<List<Job>> GetGithub(SearchViewModel viewModel)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var url = "https://jobs.github.com/positions.json?description=";
            if (viewModel.Description != null)
                url += viewModel.Description;
            url += "&location=";
            if (viewModel.Location != null)
                url += viewModel.Location;
            if (viewModel.FullTimeOnly)
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
            foreach (var job in githubJobs)
            {
                results.Add(new Job
                {
                    Title = job.Title,
                    Description = Regex.Replace(job.DescriptionHTML, "<.*?>", string.Empty),
                    CreatedAt = job.GetDateTime(),
                    Company = job.Company,
                    Location = job.Location,
                    URL = job.URL
                });
            }

            return results;
        }
    }
}
