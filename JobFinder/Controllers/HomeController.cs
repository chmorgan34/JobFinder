using JobFinder.Models;
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
                var results = new List<Job>();

                if (searchVM.Adzuna)
                    results.AddRange(await apiHelper.GetAdzuna(searchVM));
                if (searchVM.Github)
                    results.AddRange(await apiHelper.GetGithub(searchVM));
                if (searchVM.Reed)
                    results.AddRange(await apiHelper.GetReed(searchVM));

                if (searchVM.SortBy == "date")
                    results.Sort((x, y) => y.CreatedAt.CompareTo(x.CreatedAt));
                else if (searchVM.SortBy == "salary")
                    results.Sort((x, y) => Nullable.Compare(y.MaxSalary, x.MaxSalary));

                searchVM.Results = results;
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
