using JobFinder.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JobFinder.ViewModels
{
    public class JobTableRowViewModel
    {
        public Job Job { get; set; }


        [Display(Name = "Saved")]
        public bool SavedCheck { get; set; }

        [Display(Name = "Applied")]
        public bool AppliedCheck { get; set; }

        [Display(Name = "Interviewing")]
        public bool InterviewingCheck { get; set; }

        [Display(Name = "Offered")]
        public bool OfferedCheck { get; set; }
    }
}
