using JobFinder.Data;
using JobFinder.Models;
using JobFinder.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobFinder.Controllers
{
    [Authorize]
    public class MyJobsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public MyJobsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> ChangeSaveCategory(JobTableRowViewModel tableRowVM)
        {
            var user = await userManager.GetUserAsync(User);
            user = await context.GetUserWithJobsAsync(user, tracking: true);

            var job = await context.Jobs
                .Include(j => j.UsersSavedBy)
                .Include(j => j.UsersAppliedBy)
                .Include(j => j.UsersInterviewing)
                .Include(j => j.UsersOfferedTo)
                .SingleOrDefaultAsync(j => j.Equals(tableRowVM.Job));

            if (job == null)
                job = tableRowVM.Job;


            if (user.SavedJobs.Contains(job))
            {
                if (!tableRowVM.SavedCheck)
                    user.SavedJobs.Remove(job);
            }
            else if (tableRowVM.SavedCheck)
                user.SavedJobs.Add(job);


            if (user.JobsAppliedTo.Contains(job))
            {
                if (!tableRowVM.AppliedCheck)
                    user.JobsAppliedTo.Remove(job);
            }
            else if (tableRowVM.AppliedCheck)
                user.JobsAppliedTo.Add(job);


            if (user.JobsInterviewingWith.Contains(job))
            {
                if (!tableRowVM.InterviewingCheck)
                    user.JobsInterviewingWith.Remove(job);
            }
            else if (tableRowVM.InterviewingCheck)
                user.JobsInterviewingWith.Add(job);


            if (user.JobsOffered.Contains(job))
            {
                if (!tableRowVM.OfferedCheck)
                    user.JobsOffered.Remove(job);
            }
            else if (tableRowVM.OfferedCheck)
                user.JobsOffered.Add(job);


            context.SaveChanges();
            return Ok();
        }

        public async Task<IActionResult> Saved()
        {
            var user = await userManager.GetUserAsync(User);
            user = await context.GetUserWithJobsAsync(user, tracking: false);

            var tableRows = new List<JobTableRowViewModel>();
            foreach (var job in user.SavedJobs)
            {
                tableRows.Add(new JobTableRowViewModel()
                {
                    Job = job,
                    SavedCheck = true,
                    AppliedCheck = user.JobsAppliedTo.Contains(job),
                    InterviewingCheck = user.JobsInterviewingWith.Contains(job),
                    OfferedCheck = user.JobsOffered.Contains(job)
                });
            }

            ViewData["MyJobsNav"] = "Saved";
            return View("MyJobs", tableRows);
        }

        public async Task<IActionResult> Applied()
        {
            var user = await userManager.GetUserAsync(User);
            user = await context.GetUserWithJobsAsync(user, tracking: false);

            var tableRows = new List<JobTableRowViewModel>();
            foreach (var job in user.JobsAppliedTo)
            {
                tableRows.Add(new JobTableRowViewModel()
                {
                    Job = job,
                    SavedCheck = user.SavedJobs.Contains(job),
                    AppliedCheck = true,
                    InterviewingCheck = user.JobsInterviewingWith.Contains(job),
                    OfferedCheck = user.JobsOffered.Contains(job)
                });
            }

            ViewData["MyJobsNav"] = "Applied";
            return View("MyJobs", tableRows);
        }

        public async Task<IActionResult> Interviewing()
        {
            var user = await userManager.GetUserAsync(User);
            user = await context.GetUserWithJobsAsync(user, tracking: false);

            var tableRows = new List<JobTableRowViewModel>();
            foreach (var job in user.JobsInterviewingWith)
            {
                tableRows.Add(new JobTableRowViewModel()
                {
                    Job = job,
                    SavedCheck = user.SavedJobs.Contains(job),
                    AppliedCheck = user.JobsAppliedTo.Contains(job),
                    InterviewingCheck = true,
                    OfferedCheck = user.JobsOffered.Contains(job)
                });
            }

            ViewData["MyJobsNav"] = "Interviewing";
            return View("MyJobs", tableRows);
        }

        public async Task<IActionResult> Offered()
        {
            var user = await userManager.GetUserAsync(User);
            user = await context.GetUserWithJobsAsync(user, tracking: false);

            var tableRows = new List<JobTableRowViewModel>();
            foreach (var job in user.JobsOffered)
            {
                tableRows.Add(new JobTableRowViewModel()
                {
                    Job = job,
                    SavedCheck = user.SavedJobs.Contains(job),
                    AppliedCheck = user.JobsAppliedTo.Contains(job),
                    InterviewingCheck = user.JobsInterviewingWith.Contains(job),
                    OfferedCheck = true
                });
            }

            ViewData["MyJobsNav"] = "Offered";
            return View("MyJobs", tableRows);
        }
    }
}
