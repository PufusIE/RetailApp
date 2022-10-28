using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RADataManagerLibrary.DataAccess;
using RAApi.Models;
using System.Security.Claims;
using RADataManagerLibrary.Models;
using RAApi.Data;

namespace RAApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {        
        private readonly IUserData _userData;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserData userData,
                              ApplicationDbContext context,
                              UserManager<IdentityUser> userManager,
                              ILogger<UserController> logger)
        {
            _userData = userData;
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        //This is called using logged on user credentials, means you don't pass a value manually
        [HttpGet] // GET /api/User
        public UserModel GetById()
        {            
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return _userData.GetUserById(userId).First();
        }

        public record UserRegistrationModel(
            string FirstName, 
            string LastName, 
            string EmailAddress, 
            string Password);

        [HttpPost]
        [Route("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserRegistrationModel user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(user.EmailAddress);
                if (existingUser is null)
                {
                    IdentityUser newUser = new IdentityUser
                    {
                        Email = user.EmailAddress,
                        EmailConfirmed = true, // Add email verification at the production stage
                        UserName = user.EmailAddress
                    };

                    IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);

                    if (result.Succeeded)
                    {
                        existingUser = await _userManager.FindByEmailAsync(user.EmailAddress);

                        if (existingUser is null)
                        {
                            return BadRequest();
                        }

                        UserModel u = new()
                        {
                            Id = existingUser.Id,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            EmailAddress = user.EmailAddress
                        };

                        _userData.CreateUser(u);
                        return Ok();
                    }
                }
            }

            return BadRequest();
        }

        //Returns info about all registered users and their roles
        [Authorize(Roles = "Admin")]        
        [HttpGet]
        [Route("Admin/GetAllUsers")]
        public List<ApplicationUserModel> GetAllUsers()
        {
            List<ApplicationUserModel> output = new List<ApplicationUserModel>();                      

            output = _context.Users.ToList().Select(u => new ApplicationUserModel
            {
                Id = u.Id,
                Email = u.Email,
                Roles = (from ur in _context.UserRoles
                         join r in _context.Roles on ur.RoleId equals r.Id
                         where ur.UserId == u.Id
                         select new { ur.RoleId, r.Name}).ToDictionary(x => x.RoleId, x => x.Name)
            }).ToList();         

            return output;
        }

        [Authorize(Roles = "Admin")]        
        [HttpGet]
        [Route("Admin/GetAllRoles")]
        public Dictionary<string, string> GetAllRoles()
        {
            var output = _context.Roles.ToDictionary(x => x.Id, x => x.Name);

            return output;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/AddRole")]
        public async Task AddARole(UserRolePairModel pairing)
        {
            string LoggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userManager.FindByIdAsync(pairing.UserId);

            _logger.LogInformation("Admin {Admin} added user {User} to role {Role}",
                LoggedInUserId, user.Id, pairing.RoleName);

            await _userManager.AddToRoleAsync(user, pairing.RoleName);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/RemoveRole")]
        public async Task RemoveARole(UserRolePairModel pairing)
        {
            string LoggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userManager.FindByIdAsync(pairing.UserId);

            _logger.LogInformation("Admin {Admin} removed user {User} from role {Role}",
                LoggedInUserId, user.Id, pairing.RoleName);

            await _userManager.RemoveFromRoleAsync(user, pairing.RoleName);
        }
    }
}
