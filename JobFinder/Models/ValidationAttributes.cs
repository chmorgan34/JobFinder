using JobFinder.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JobFinder.Models
{
    public class JoobleKeywordValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var searchVM = (SearchViewModel)validationContext.ObjectInstance;
            var keywords = (string)value;

            if (searchVM.JoobleCheck && (keywords == null || keywords == string.Empty))
                return new ValidationResult("Jooble requires at least one keyword.");
            else
                return ValidationResult.Success;
        }
    }
}
