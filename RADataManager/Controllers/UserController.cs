using Microsoft.AspNet.Identity;
using RADataManagerLibrary.DataAccess;
using RADataManagerLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace RADataManager.Controllers
{
    [Authorize]
    public class UserController : ApiController
    {        
        public List<UserModel> GetById()
        {
            UserData data = new UserData();
            string userId = RequestContext.Principal.Identity.GetUserId();

            return data.GetUserById(userId);
        }
    }
}
