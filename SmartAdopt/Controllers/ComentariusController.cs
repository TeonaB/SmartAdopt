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
                // Verify the Postare exists
                var postare = await db.Postares.FindAsync(id);
                if (postare == null)
                {
                    TempData["message"] = "Postarea nu a fost găsită.";
                    TempData["messageType"] = "error";
                    return RedirectToAction("Index", "Postares");
                }

                // Set properties
                comentariu.idPostare = id;
                var user = await _userManager.GetUserAsync(User);
                var client = await db.Clients.FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);
                comentariu.idClient = client.idClient;

                // Add the comment to the database
                db.Comentarius.Add(comentariu);
                await db.SaveChangesAsync();
                return RedirectToAction("Show", "Postares", new { id = id });
            }
            catch (Exception)
            {
                TempData["message"] = "A apărut o eroare la adăugarea comentariului";
                TempData["messageType"] = "error";
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
                TempData["message"] = "Comentariul nu a fost găsit";
                return RedirectToAction("Index", "Animals");
            }

            try
            {
                db.Comentarius.Remove(comentariu);
                await db.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                TempData["message"] = "A apărut o eroare la ștergerea comentariului: " + ex.Message;
            }

            return RedirectToAction("Show", "Postares", new { id = id2});
        }
    }
}
