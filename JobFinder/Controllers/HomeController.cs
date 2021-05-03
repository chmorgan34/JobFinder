using JobFinder.Data;
using JobFinder.Models;
using JobFinder.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IApiHelper apiHelper;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            IApiHelper apiHelper)
        {
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.apiHelper = apiHelper;
        }

        [Route("")]
        public IActionResult Index() => View(new SearchViewModel());

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
                if (searchVM.GithubjobsCheck)
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

                searchVM.FailedRequests = failedRequests;


                var user = await userManager.GetUserAsync(User);
                if (signInManager.IsSignedIn(User))
                    user = await context.GetUserWithJobsAsync(user, tracking: false);

                var tableRows = new List<JobTableRowViewModel>();
                for (int i = 0; i < results.Count; i++)
                {
                    var isSaved = false;
                    var isApplied = false;
                    var isInterviewing = false;
                    var isOffered = false;

                    if (signInManager.IsSignedIn(User))
                    {
                        foreach (var savedJob in user.SavedJobs)
                        {
                            if (savedJob.Equals(results[i]))
                            {
                                isSaved = true;
                                results[i] = savedJob;
                                break;
                            }
                        }
                        foreach (var appliedJob in user.JobsAppliedTo)
                        {
                            if (appliedJob.Equals(results[i]))
                            {
                                isApplied = true;
                                results[i] = appliedJob;
                                break;
                            }
                        }
                        foreach (var interviewingJob in user.JobsInterviewingWith)
                        {
                            if (interviewingJob.Equals(results[i]))
                            {
                                isInterviewing = true;
                                results[i] = interviewingJob;
                                break;
                            }
                        }
                        foreach (var offeredJob in user.JobsOffered)
                        {
                            if (offeredJob.Equals(results[i]))
                            {
                                isOffered = true;
                                results[i] = offeredJob;
                                break;
                            }
                        }
                    }

                    tableRows.Add(new JobTableRowViewModel()
                    {
                        Job = results[i],
                        SavedCheck = isSaved,
                        AppliedCheck = isApplied,
                        InterviewingCheck = isInterviewing,
                        OfferedCheck = isOffered
                    });
                }

                if (searchVM.SortBy == "date")
                {
                    if (searchVM.SortDirection == "up")
                        tableRows.Sort((x, y) => x.Job.CreatedAt.CompareTo(y.Job.CreatedAt));
                    else if (searchVM.SortDirection == "down")
                        tableRows.Sort((x, y) => y.Job.CreatedAt.CompareTo(x.Job.CreatedAt));
                }
                else if (searchVM.SortBy == "salary")
                {
                    if (searchVM.SortDirection == "up")
                        tableRows.Sort((x, y) => Nullable.Compare(x.Job.MinSalary, y.Job.MinSalary));
                    else if (searchVM.SortDirection == "down")
                        tableRows.Sort((x, y) => Nullable.Compare(y.Job.MinSalary, x.Job.MinSalary));
                }

                searchVM.Results = tableRows;
            }

            return View("Index", searchVM);
        }


        [Route("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => 
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
