using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RAWPFDesktopUILibrary.Models
{
    public class CreateUserModel
    {
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [DisplayName("Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        // TODO: fix - [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Password must be at least 8 characters long and contain number, uppercase and lowercase character")]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password), ErrorMessage ="The passwords do not match")]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
