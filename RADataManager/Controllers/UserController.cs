using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RADataManager.Models;
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
        //This is called using logged on user credentials, means you don't pass a value manually
        [HttpGet] // GET /api/User
        public UserModel GetById()
        {
            UserData data = new UserData();
            // Gets user id by bearer token
            string userId = RequestContext.Principal.Identity.GetUserId();

            return data.GetUserById(userId).First();
        }

        //Returns info about all registered users
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("api/User/Admin/GetAllUsers")]
        public List<ApplicationUserModel> GetAllUsers()
        {
            List<ApplicationUserModel> output = new List<ApplicationUserModel>();
            
            //Entity Framework
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                //Gets users
                var users = userManager.Users.ToList();
                //Gets roels
                var roles = context.Roles.ToList();

                //Overcomplicated LINQ that I don't really like, could've done the same with two foreach
                output = users.Select(u => new ApplicationUserModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    Roles = u.Roles.Join(roles,
                                        s1 => s1.RoleId,
                                        s2 => s2.Id,
                                        (s1, s2) => new { a = s1, b = s2 }).ToDictionary(x => x.a.RoleId, x => x.b.Name)
                }).ToList();
            }

            return output;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("api/User/Admin/GetAllRoles")]
        public Dictionary<string, string> GetAllRoles()
        {
            using (var context = new ApplicationDbContext())
            {
                var output = context.Roles.ToDictionary(x => x.Id, x => x.Name);

                return output;
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("api/User/Admin/AddRole")]
        public void AddARole(UserRolePairModel pairing)
        {
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                userManager.AddToRole(pairing.UserId, pairing.RoleName);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("api/User/Admin/RemoveRole")]
        public void RemoveARole(UserRolePairModel pairing)
        {
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                userManager.RemoveFromRole(pairing.UserId, pairing.RoleName);
            }
        }
    }
}
