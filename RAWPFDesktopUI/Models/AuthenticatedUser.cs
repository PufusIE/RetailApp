using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWPFDesktopUI.Models
{
    public class AuthenticatedUser
    {
        //Models getting populated by caliburn.micro, so names should match the api response model
        public string Access_Token { get; set; }
        public string Username { get; set; }
    }
}
