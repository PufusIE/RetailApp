using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RAApi.Models;
using System.Diagnostics;

namespace RAApi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            ////one time use, temporary hard code role insert
            //string[] roles = { "Admin", "Manager", "Cashier" };

            //foreach (var role in roles)
            //{
            //    var roleExist = await _roleManager.RoleExistsAsync(role);

            //    if (roleExist == false)
            //    {
            //        await _roleManager.CreateAsync(new IdentityRole(role));
            //    }
            //}

            //var user = await _userManager.FindByEmailAsync("email");

            //if (user != null)
            //{
            //    await _userManager.AddToRoleAsync(user, "role");
            //}

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}