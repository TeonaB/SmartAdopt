using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartAdopt.Data;
using SmartAdopt.Models;

namespace SmartAdopt.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext db;
        private IWebHostEnvironment _env;

        public AdminsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment env)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }
            var admin = await db.Admins.FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            ViewBag.AnimalCount = await db.Animals.CountAsync();
            ViewBag.ComenziCount = await db.Comandas.CountAsync();

            return View(user);
        }

        public IActionResult AddAnimal()
        {
            return View(new Animal());
        }

        private string databaseFileName;
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAnimal(Animal animal, IFormFile? imageFile)
        {
            // Verificam daca exista imaginea in request 
            if (imageFile != null && imageFile.Length > 0)
            {
                // Generam calea de stocare a fisierului
                var storagePath = Path.Combine(
                _env.WebRootPath, // Preluam calea folderului wwwroot
                "images", // Adaugam calea folderului images
                imageFile.FileName // Numele fisierului
                );

                // Generam calea de afisare a fisierului care va fi stocata in baza de date
                databaseFileName = "/images/" + imageFile.FileName;

                // Uploadam fisierul la calea de storage
                using (var fileStream = new FileStream(storagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }
            }
            else
            {
                databaseFileName = "/images/logo.png";
            }

            // Salvam storagePath-ul in baza de date
            animal.ImagePath = databaseFileName;
            db.Animals.Add(animal);
            await db.SaveChangesAsync();
            TempData["message"] = "Animalutul a fost adaugat";
            return RedirectToAction("Index", "Animals");

        }

        public async Task<IActionResult> DeleteAnimal(int id)
        {
            
            var animal = await db.Animals.FindAsync(id);
            if (animal == null)
            {
                TempData["message"] = "Animalul nu a fost găsit.";
                return RedirectToAction("Index", "Animals");
            }

            bool hasOrders = await db.Comandas.AnyAsync(c => c.idAnimal == id);
            if (hasOrders)
            {
                TempData["message"] = "Animalul nu poate fi șters deoarece are comenzi asociate.";
                return RedirectToAction("Index", "Animals");
            }

            try
            {
                db.Animals.Remove(animal);
                await db.SaveChangesAsync();

                TempData["message"] = "Animalul a fost șters cu succes.";
            }
            catch (Exception ex)
            {
                TempData["message"] = "A apărut o eroare la ștergerea animalului: " + ex.Message;
            }

            return RedirectToAction("Index", "Animals");
        }
    }
}
