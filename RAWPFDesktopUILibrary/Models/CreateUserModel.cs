using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RAWPFDesktopUILibrary.Models
{
    public class CreateUserModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        [RegularExpression(@"[A-Z][a-z][0-9]|.{8,}", 
        ErrorMessage = "Password must be at least 8 characters long and contain number, uppercase and lowercase character")]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password), ErrorMessage ="The passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
