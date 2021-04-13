using JobFinder.Models;
using JobFinder.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace JobFinder.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly IApiHelper apiHelper;

        public HomeController(IApiHelper apiHelper)
        {
            this.apiHelper = apiHelper;
        }

        [Route("")]
        public IActionResult Index()
        {
            return View(new SearchViewModel());
        }

        [Route("Search")]
        public async Task<IActionResult> Search(SearchViewModel searchVM)
        {
            if (ModelState.IsValid)
            {
                var results = new List<Job>();
                var failedRequests = new List<string>();

                if (searchVM.AdzunaCheck)
                {
                    try
                    {
                        results.AddRange(await apiHelper.GetAdzunaAsync(searchVM));
                    }
                    catch (HttpRequestException)
                    {
                        failedRequests.Add("Adzuna");
                    }
                }
                if (searchVM.GithubCheck)
                {
                    try
                    {
                        results.AddRange(await apiHelper.GetGithubjobsAsync(searchVM));
                    }
                    catch (HttpRequestException)
                    {
                        failedRequests.Add("GitHub Jobs");
                    }
                }
                if (searchVM.JoobleCheck)
                {
                    try
                    {
                        results.AddRange(await apiHelper.GetJoobleAsync(searchVM));
                    }
                    catch (HttpRequestException)
                    {
                        failedRequests.Add("Jooble");
                    }
                }
                if (searchVM.ThemuseCheck)
                {
                    try
                    {
                        results.AddRange(await apiHelper.GetThemuseAsync(searchVM));
                    }
                    catch (HttpRequestException)
                    {
                        failedRequests.Add("The Muse");
                    }
                }
                if (searchVM.ReedCheck)
                {
                    try
                    {
                        results.AddRange(await apiHelper.GetReedAsync(searchVM));
                    }
                    catch (HttpRequestException)
                    {
                        failedRequests.Add("Reed");
                    }
                }
                if (searchVM.UsajobsCheck)
                {
                    try
                    {
                        results.AddRange(await apiHelper.GetUsajobsAsync(searchVM));
                    }
                    catch (HttpRequestException)
                    {
                        failedRequests.Add("USAJOBS");
                    }
                }

                if (searchVM.SortBy == "date")
                {
                    if (searchVM.SortDirection == "up")
                        results.Sort((x, y) => x.CreatedAt.CompareTo(y.CreatedAt));
                    else if (searchVM.SortDirection == "down")
                        results.Sort((x, y) => y.CreatedAt.CompareTo(x.CreatedAt));
                }
                else if (searchVM.SortBy == "salary")
                {
                    if (searchVM.SortDirection == "up")
                        results.Sort((x, y) => Nullable.Compare(x.MinSalary, y.MinSalary));
                    else if (searchVM.SortDirection == "down")
                        results.Sort((x, y) => Nullable.Compare(y.MinSalary, x.MinSalary));
                }

                searchVM.Results = results;
                searchVM.FailedRequests = failedRequests;
            }

            return View("Index", searchVM);
        }

        [Route("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
