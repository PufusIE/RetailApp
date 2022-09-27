﻿using System;

namespace RAWPFDesktopUILibrary.Models
{
    public interface ILoggedInUser
    {
        DateTime CreateDate { get; set; }
        string EmailAddress { get; set; }
        string FirstName { get; set; }
        string Id { get; set; }
        string LastName { get; set; }
        string Token { get; set; }
    }
}