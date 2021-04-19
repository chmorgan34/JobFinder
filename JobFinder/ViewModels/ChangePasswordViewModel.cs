using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JobFinder.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "You forgot something...")]
        [DataType(DataType.Password)]
        [Display(Prompt = "Current password")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "You forgot something...")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Must be between {2}-{1} characters.")]
        [Display(Prompt = "New password")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "You forgot something...")]
        [Compare("NewPassword", ErrorMessage = "New passwords do not match.")]
        [DataType(DataType.Password)]
        [Display(Prompt = "Confirm new password")]
        public string ConfirmNewPassword { get; set; }

        public bool Success { get; set; } = false;
    }
}
