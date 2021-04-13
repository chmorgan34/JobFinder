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
        // -------------------- Output --------------------
        public List<Job> Results { get; set; }
        public List<string> FailedRequests { get; set; }


        // -------------------- Input ---------------------

        // Job boards
        [Display(Name = "Adzuna")]
        public bool AdzunaCheck { get; set; } = true;

        [Display(Name = "GitHub Jobs")]
        public bool GithubCheck { get; set; } = true;

        [Display(Name = "Jooble")]
        public bool JoobleCheck { get; set; } = true;

        [Display(Name = "The Muse")]
        public bool ThemuseCheck { get; set; } = true;

        [Display(Name = "USAJOBS")]
        public bool UsajobsCheck { get; set; } = true;

        [Display(Name = "Reed")]
        public bool ReedCheck { get; set; } = false;


        // Search parameters
        [Required]
        [Display(Name = "Country")]
        public string AdzunaCountry { get; set; } = "us";

        [Display(Name = "Category")]
        public string ThemuseCategory { get; set; } = "Software Engineer";

        [KeywordsValidation]
        public string Keywords { get; set; }

        public string Location { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Can't be less than 1.")]
        [Display(Name = "Distance (in miles)")]
        public int? MilesAway { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Can't be less than 0.")]
        [Display(Name = "Minimum salary")]
        public int? MinSalary { get; set; }

        [Display(Name = "Full-time only")]
        public bool FullTimeOnly { get; set; } = true;

        [Display(Name = "Permanent only")]
        public bool PermanentOnly { get; set; } = true;

        [Display(Name = "Sort by")]
        public string SortBy { get; set; } = "relevance";

        public string SortDirection { get; set; } = "down";

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Can't be less than 1.")]
        public int Page { get; set; } = 1;

        // <select> items
        public List<SelectListItem> Countries { get; } = new()
        {
            new SelectListItem { Value = "us", Text = "United States" },
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
        public List<SelectListItem> ThemuseCategories { get; } = new()
        {
            new SelectListItem { Value = "Accounting", Text = "Accounting" },
            new SelectListItem { Value = "Corporate", Text = "Corporate" },
            new SelectListItem { Value = "Data Science", Text = "Data Science" },
            new SelectListItem { Value = "Editor", Text = "Editor" },
            new SelectListItem { Value = "HR", Text = "HR" },
            new SelectListItem { Value = "Law", Text = "Law" },
            new SelectListItem { Value = "Mechanic", Text = "Mechanic" },
            new SelectListItem { Value = "Nurses", Text = "Nurses" },
            new SelectListItem { Value = "Physical Assistant", Text = "Physical Assistant" },
            new SelectListItem { Value = "Project Management", Text = "Project Management" },
            new SelectListItem { Value = "Recruiting", Text = "Recruiting" },
            new SelectListItem { Value = "Sales", Text = "Sales" },
            new SelectListItem { Value = "UX", Text = "UX" },
            new SelectListItem { Value = "Writer", Text = "Writer" },
            new SelectListItem { Value = "Account Management/Customer Success", Text = "Account Management/Customer Success" },
            new SelectListItem { Value = "Customer Service Career", Text = "Customer Service Career" },
            new SelectListItem { Value = "Design", Text = "Design" },
            new SelectListItem { Value = "Education", Text = "Education" },
            new SelectListItem { Value = "IT", Text = "IT" },
            new SelectListItem { Value = "Marketing", Text = "Marketing" },
            new SelectListItem { Value = "Mental Health", Text = "Mental Health" },
            new SelectListItem { Value = "Office Administration", Text = "Office Administration" },
            new SelectListItem { Value = "Product", Text = "Product" },
            new SelectListItem { Value = "Public Relations", Text = "Public Relations" },
            new SelectListItem { Value = "Retail", Text = "Retail" },
            new SelectListItem { Value = "Software Engineer", Text = "Software Engineer" },
            new SelectListItem { Value = "Videography", Text = "Videography" }
        };
        public List<SelectListItem> SortTypes { get; } = new()
        {
            new SelectListItem { Value = "relevance", Text = "Relevance" },
            new SelectListItem { Value = "date", Text = "Date" },
            new SelectListItem { Value = "salary", Text = "Salary" }
        };
        public List<SelectListItem> SortDirections { get; } = new()
        {
            new SelectListItem { Value = "up", Text = "Ascending" },
            new SelectListItem { Value = "down", Text = "Descending" }
        };
    }
}
