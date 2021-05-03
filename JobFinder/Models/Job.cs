using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace JobFinder.Models
{
    public enum JobSchedule { FullTime, PartTime }
    public enum EmploymentLength { Permanent, Temporary }

    public class Job
    {
        public int ID { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public string URL { get; set; }

        public int? MinSalary { get; set; }
        public int? MaxSalary { get; set; }
        public string CultureName { get; set; }
        public JobSchedule? Schedule { get; set; }
        public EmploymentLength? EmploymentType { get; set; }

        public HashSet<ApplicationUser> UsersSavedBy { get; set; } = new();
        public HashSet<ApplicationUser> UsersAppliedBy { get; set; } = new();
        public HashSet<ApplicationUser> UsersInterviewing { get; set; } = new();
        public HashSet<ApplicationUser> UsersOfferedTo { get; set; } = new();


        public string GetScheduleString()
        {
            switch (Schedule)
            {
                case JobSchedule.FullTime:
                    return "Full-time";
                case JobSchedule.PartTime:
                    return "Part-time";
                default:
                    return null;
            }
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !GetType().Equals(obj.GetType()))
                return false;
            else
            {
                Job j = (Job) obj;
                return URL == j.URL;
            }
        }

        public override int GetHashCode()
        {
            return URL.GetHashCode();
        }
    }
}
