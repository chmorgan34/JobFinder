using JobFinder.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JobFinder.ViewModels
{
    public class SearchViewModel
    {
        public List<Job> Results { get; } = new List<Job>();


        [Display(Name = "Adzuna")]
        public bool AdzunaChecked { get; set; } = true;

        [Display(Name = "GitHub Jobs")]
        public bool GithubChecked { get; set; } = true;

        [Display(Name = "Country")]
        public string Country { get; set; } = "us";

        [Display(Name = "Description")]
        public string Description { get; set; } = null;

        [Display(Name = "Location")]
        public string Location { get; set; } = null;

        [Range(1, int.MaxValue, ErrorMessage = "Distance must be at least 1")]
        [Display(Name = "Distance (in kilometers)")]
        public int? Distance { get; set; } = null;

        [Range(0, int.MaxValue, ErrorMessage = "Minimum salary can't be less than 0")]
        [Display(Name = "Minimum salary")]
        public int? MinSalary { get; set; } = null;

        [Range(0, int.MaxValue, ErrorMessage = "Max days old can't be less than 0")]
        [Display(Name = "Max days old")]
        public int? MaxDaysOld { get; set; } = null;

        [Display(Name = "Full-time only")]
        public bool FullTimeOnlyChecked { get; set; } = true;

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;


        public List<SelectListItem> Countries { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "us", Text = "United States"},
            new SelectListItem { Value = "at", Text = "Austria" },
            new SelectListItem { Value = "br", Text = "Brazil" },
            new SelectListItem { Value = "ca", Text = "Canada" },
            new SelectListItem { Value = "de", Text = "Germany" },
            new SelectListItem { Value = "fr", Text = "France" },
            new SelectListItem { Value = "in", Text = "India" },
            new SelectListItem { Value = "it", Text = "Italy" },
            new SelectListItem { Value = "nl", Text = "Netherlands" },
            new SelectListItem { Value = "nz", Text = "New Zealand" },
            new SelectListItem { Value = "pl", Text = "Poland" },
            new SelectListItem { Value = "ru", Text = "Russia" },
            new SelectListItem { Value = "sg", Text = "Singapore" },
            new SelectListItem { Value = "za", Text = "South Africa" },
            new SelectListItem { Value = "gb", Text = "United Kingdom" }
        };
    }
}
