using System;
using System.ComponentModel.DataAnnotations;

namespace Develover.WebUI.Models
{
    public class ChangePasswordViewModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        [Display(Name = "Password")]
        [Required]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Display(Name = "New password")]
        [Required]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Confirm new password")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public string ReturnUrl { get; set; }
    }
}
