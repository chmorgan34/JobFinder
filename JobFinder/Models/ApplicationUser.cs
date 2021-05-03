using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobFinder.Models
{
    public class ApplicationUser : IdentityUser
    {
        public HashSet<Job> SavedJobs { get; set; } = new();
        public HashSet<Job> JobsAppliedTo { get; set; } = new();
        public HashSet<Job> JobsInterviewingWith { get; set; } = new();
        public HashSet<Job> JobsOffered { get; set; } = new();
    }
}
