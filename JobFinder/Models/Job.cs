using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobFinder.Models
{
    public enum JobSchedule { FullTime, PartTime }
    public enum EmploymentType { Permanent, Temporary }

    public class Job
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public string URL { get; set; }

        public int? MinSalary { get; set; }
        public int? MaxSalary { get; set; }
        public JobSchedule? Schedule { get; set; }
        public EmploymentType? EmploymentType { get; set; }


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
    }
}
