using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobFinder.Models
{
    public enum ContractTime { FullTime, PartTime }
    public enum ContractType { Permanent, Temporary }

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
        public ContractTime? ContractTime { get; set; }
        public ContractType? ContractType { get; set; }


        public string GetContractTime()
        {
            switch (ContractTime)
            {
                case Models.ContractTime.FullTime:
                    return "Full-time";
                case Models.ContractTime.PartTime:
                    return "Part-time";
                default:
                    return null;
            }
        }
    }
}
