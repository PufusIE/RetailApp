﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWPFDesktopUILibrary.Models
{
    public class LoggedInUserModel : ILoggedInUserModel
    {
        public string Token { get; set; }
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public DateTime CreateDate { get; set; }

        public void ResetUserModel()
        {
            Token = string.Empty;
            Id = string.Empty ;
            FirstName = string.Empty;
            LastName = string.Empty;
            EmailAddress = string.Empty;
            CreateDate = DateTime.MinValue;
        }
    }
}
