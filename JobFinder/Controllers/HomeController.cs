﻿using JobFinder.Models;
using JobFinder.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                var country = searchVM.Countries.Find(sli => sli.Value == searchVM.Country).Text;
                if (searchVM.Location == null)
                    searchVM.Location = country;
                else searchVM.Location += ", " + country;

                var results = new List<Job>();
                var errors = new List<string>();

                if (searchVM.Adzuna)
                {
                    try
                    {
                        results.AddRange(await apiHelper.GetAdzunaAsync(searchVM));
                    }
                    catch
                    {
                        errors.Add("Adzuna");
                    }
                }
                if (searchVM.Github)
                {
                    try
                    {
                        results.AddRange(await apiHelper.GetGithubjobsAsync(searchVM));
                    }
                    catch
                    {
                        errors.Add("GitHub Jobs");
                    }
                }
                if (searchVM.Jooble)
                {
                    try
                    {
                        results.AddRange(await apiHelper.GetJoobleAsync(searchVM));
                    }
                    catch
                    {
                        errors.Add("Jooble");
                    }
                }
                if (searchVM.Reed)
                {
                    try
                    {
                        results.AddRange(await apiHelper.GetReedAsync(searchVM));
                    }
                    catch
                    {
                        errors.Add("Reed");
                    }
                }
                if (searchVM.Usajobs)
                {
                    try
                    {
                        results.AddRange(await apiHelper.GetUsajobsAsync(searchVM));
                    }
                    catch
                    {
                        errors.Add("USAJOBS");
                    }
                }

                if (searchVM.SortBy == "date")
                    results.Sort((x, y) => y.CreatedAt.CompareTo(x.CreatedAt));
                else if (searchVM.SortBy == "salary")
                    results.Sort((x, y) => Nullable.Compare(y.MinSalary, x.MinSalary));

                searchVM.Results = results;
                searchVM.Errors = errors;
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
