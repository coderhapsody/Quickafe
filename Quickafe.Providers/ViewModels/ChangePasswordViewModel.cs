using System.ComponentModel.DataAnnotations;

namespace Quickafe.Providers.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Invalid Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
