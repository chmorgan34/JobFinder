using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JobFinder.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "You forgot something...")]
        public string Username { get; set; }

        [Required(ErrorMessage = "You forgot something...")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Must be between {2}-{1} characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "You forgot something...")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }


        // Hidden
        public string ReturnUrl { get; set; }
    }
}
