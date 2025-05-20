using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartAdopt.Data;
using SmartAdopt.Models;

namespace SmartAdopt.Controllers
{
    [Authorize]
    public class ClientsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext db;

        public ClientsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }
            var client = await db.Clients.FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            if (client.CompletedProfile == false)
            {
                if (TempData.TryGetValue("ErrorMessage", out object? value))
                {
                    ViewBag.Message = value;
                    return View(user);
                }
                else
                {
                    TempData["ErrorMessage"] = "Va rugam sa va completati tot profilul!";
                    ViewBag.Message = TempData["ErrorMessage"];
                    return View(user);
                }
            }

            if (TempData.ContainsKey("SuccessMessage"))
            {
                ViewBag.Message = TempData["SuccessMessage"];
                return View(user);
            }
            return View(user);
        }
    }
}
