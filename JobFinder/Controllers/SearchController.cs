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
    public class SearchController : Controller
    {
        private readonly IApiHelper apiHelper;

        public SearchController(IApiHelper apiHelper)
        {
            this.apiHelper = apiHelper;
        }

        [Route("")]
        public IActionResult Search()
        {
            return View(new SearchViewModel());
        }

        [Route("Search")]
        [HttpPost]
        public async Task<IActionResult> Search(SearchViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.AdzunaChecked)
                    viewModel.Results.AddRange(await apiHelper.GetAdzuna(viewModel));

            }

            return View(viewModel);
        }

        [Route("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
