using JobFinder.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JobFinder.Models
{
    public class CountryValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var searchVM = (SearchViewModel)validationContext.ObjectInstance;
            var countryAbbr = (string)value;
            var country = searchVM.Countries.Find(sli => sli.Value == countryAbbr).Text;

            if (searchVM.Usajobs && countryAbbr != "us")
                return new ValidationResult($"USAJOBS doesn't provide jobs from {country}.");
            else if (searchVM.Reed && countryAbbr != "uk")
                return new ValidationResult($"Reed doesn't provide jobs from {country}.");
            else
                return ValidationResult.Success;
        }
    }
}
