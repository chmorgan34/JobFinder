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
        public async Task<IActionResult> Search(SearchViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var results = new List<Job>();

                if (viewModel.Adzuna)
                    results.AddRange(await apiHelper.GetAdzuna(viewModel));

                viewModel.Results = results;
            }

            return View("Index", viewModel);
        }

        [Route("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
