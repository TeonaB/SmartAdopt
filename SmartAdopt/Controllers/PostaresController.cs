using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartAdopt.Data;
using SmartAdopt.Models;

namespace SmartAdopt.Controllers
{
    public class PostaresController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext db;

        public PostaresController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var search = "";
            var postari = db.Postares.Include(p => p.ApplicationUser)
                .OrderByDescending(p => p.data_postarii);
            //MOTOR DE CAUTARE
            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                // eliminam spatiile libere
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
                // Cautare in produs (Title si Description)
                List<int> postareIds = db.Postares.Where(p => p.titlu.Contains(search) || p.descriere.Contains(search)).Select(a => a.idPostare).ToList();

                // Lista produselor care contin cuvantul cautat
                postari = db.Postares.Where(postare => postareIds.Contains(postare.idPostare)).Include(p => p.ApplicationUser)
                .OrderByDescending(p => p.data_postarii);
            }

            ViewBag.SearchString = search;
            ViewBag.Postari = postari;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }

        public async Task<IActionResult> Show(int id)
        {
            var postare = await db.Postares
                .Include(p => p.ApplicationUser)
                .Include(p => p.Comentarius)
                .FirstOrDefaultAsync(p => p.idPostare == id);

            if (postare == null)
            {
                return NotFound();
            }

            return View(postare);
        }
    }
}
