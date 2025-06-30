using Microsoft.AspNetCore.Authorization;
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

        public IActionResult Index(int? page)
        {
            int pageSize = 4;
            int pageNumber = page ?? 1;
            if (pageNumber < 1) pageNumber = 1;

            var search = "";
            var postari = db.Postares.Include(p => p.ApplicationUser)
                .OrderByDescending(p => p.data_postarii);
            //searchBar
            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                //elimin spatiile
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
                //cautarea
                List<int> postareIds = db.Postares.Where(p => p.titlu.Contains(search) || p.descriere.Contains(search)).Select(a => a.idPostare).ToList();

                postari = db.Postares.Where(postare => postareIds.Contains(postare.idPostare)).Include(p => p.ApplicationUser)
                .OrderByDescending(p => p.data_postarii);
            }

            int totalItems = postari.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            if (pageNumber > totalPages && totalPages > 0) pageNumber = totalPages;

            var postari2 = postari
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.SearchString = search;
            ViewBag.Postari = postari2;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }

        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> Show(int id)
        {
            var postare = await db.Postares.Include(p => p.ApplicationUser).FirstOrDefaultAsync(p => p.idPostare == id);
            var comentarii = await db.Comentarius.Where(c => c.idPostare == id).Include(c => c.Client).ThenInclude(c => c.ApplicationUser).ToListAsync();

            if (postare == null)
            {
                return NotFound();
            }
            ViewBag.Comentarii = comentarii;
            var user = await _userManager.GetUserAsync(User);
            var client = await db.Clients.FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);
            if (client != null)
                ViewBag.UserId = client.idClient;
            else
                ViewBag.UserId = 0;
            ViewBag.NewComment = new Comentariu
            {
                idPostare = id, 
                descriere = "" 
            };
            return View(postare);
        }
    }
}
