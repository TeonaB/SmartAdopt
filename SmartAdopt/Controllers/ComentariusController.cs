using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartAdopt.Data;
using SmartAdopt.Models;

namespace SmartAdopt.Controllers
{
    [Authorize]
    public class ComentariusController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext db;

        public ComentariusController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize (Roles = "Client")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int id, Comentariu comentariu)
        {
            comentariu.descriere = comentariu.descriere;
            try
            {
                var postare = await db.Postares.FindAsync(id);
                if (postare == null)
                {
                    TempData["message"] = "Postarea nu a fost gasita.";
                    TempData["messageType"] = "error";
                    return RedirectToAction("Index", "Postares");
                }

                comentariu.idPostare = id;
                var user = await _userManager.GetUserAsync(User);
                var client = await db.Clients.FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);
                comentariu.idClient = client.idClient;

                db.Comentarius.Add(comentariu);
                await db.SaveChangesAsync();
                return RedirectToAction("Show", "Postares", new { id = id });
            }
            catch (Exception)
            {
                return RedirectToAction("Show", "Postares", new { id = id });
            }

        }

        [Authorize(Roles = "Client, Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var comentariu = await db.Comentarius.FindAsync(id);
            var id2 = comentariu.idPostare;
            if (comentariu == null)
            {
                TempData["message"] = "Comentariul nu a fost gasit";
                return RedirectToAction("Index", "Animals");
            }

            try
            {
                db.Comentarius.Remove(comentariu);
                await db.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                TempData["message"] = "A aparut o eroare la stergerea comentariului: " + ex.Message;
            }

            return RedirectToAction("Show", "Postares", new { id = id2});
        }
    }
}
