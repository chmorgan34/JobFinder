using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JobFinder.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "You forgot something...")]
        public string Username { get; set; }

        [Required(ErrorMessage = "You forgot something...")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; } = true;


        // Hidden
        public string ReturnUrl { get; set; }
    }
}
