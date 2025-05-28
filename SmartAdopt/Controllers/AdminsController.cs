using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartAdopt.Data;
using SmartAdopt.Models;
using System.Linq;

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
            ViewBag.ComenziCount = await db.Comandas.Where(c => c.stare != "Respinsă").CountAsync();

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
                "images/animals", // Adaugam calea folderului images
                imageFile.FileName // Numele fisierului
                );

                // Generam calea de afisare a fisierului care va fi stocata in baza de date
                databaseFileName = "/images/animals/" + imageFile.FileName;

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

        public async Task<IActionResult> EditAnimal(int id)
        {
            var animal = await db.Animals.FindAsync(id);
            if (animal == null)
            {
                TempData["message"] = "Animalul nu a fost găsit";
                TempData["messageType"] = "error";
                return RedirectToAction("Index", "Animals");
            }

            return View(animal);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAnimal(int id, Animal animal, IFormFile ImagePath)
        {
            if (id != animal.idAnimal)
            {
                TempData["message"] = "ID-ul animalului nu corespunde";
                TempData["messageType"] = "error";
                return RedirectToAction("Index", "Animals");
            }

            var existingAnimal = await db.Animals.AsNoTracking().FirstOrDefaultAsync(a => a.idAnimal == id);
            if (existingAnimal == null)
            {
                TempData["message"] = "Animalul nu a fost găsit";
                TempData["messageType"] = "error";
                return RedirectToAction("Index", "Animals");
            }

            if (ImagePath != null && ImagePath.Length > 0)
            {
                // Generam calea de stocare a fisierului
                var storagePath = Path.Combine(
                _env.WebRootPath, // Preluam calea folderului wwwroot
                "images/animals", // Adaugam calea folderului images
                ImagePath.FileName // Numele fisierului
                );

                // Generam calea de afisare a fisierului care va fi stocata in baza de date
                databaseFileName = "/images/animals/" + ImagePath.FileName;

                // Uploadam fisierul la calea de storage
                using (var fileStream = new FileStream(storagePath, FileMode.Create))
                {
                    await ImagePath.CopyToAsync(fileStream);
                }

                // Update the animal's imageUrl with the new path
                animal.ImagePath = databaseFileName;
            }
            else
            {
                // No new image uploaded; retain the existing imageUrl
                animal.ImagePath = existingAnimal.ImagePath;
            }

            try
            {
                db.Update(animal);
                await db.SaveChangesAsync();
                TempData["message"] = "Animalul a fost actualizat cu succes";
                TempData["messageType"] = "success";
            }
            catch (DbUpdateException)
            {
                TempData["message"] = "A apărut o eroare la actualizarea animalului";
                TempData["messageType"] = "error";
            }
            return RedirectToAction("Index", "Animals");
        }


        public async Task<IActionResult> DeleteAnimal(int id)
        {
            
            var animal = await db.Animals.FindAsync(id);
            if (animal == null)
            {
                TempData["message"] = "Animalul nu a fost găsit";
                return RedirectToAction("Index", "Animals");
            }

            var orders = await db.Comandas
                .Where(c => c.idAnimal == id)
                .ToListAsync();

            if (orders.Any())
            {
                var hasNonRejectedOrders = orders.Any(c => c.stare != "Respinsă");

                if (hasNonRejectedOrders)
                {
                    TempData["message"] = "Animalul nu poate fi șters deoarece are comenzi asociate care nu sunt respinse";
                    TempData["messageType"] = "warning";
                    return RedirectToAction("Index", "Animals");
                }
                else
                {
                    db.Comandas.RemoveRange(orders);
                }
            }

            try
            {
                db.Animals.Remove(animal);
                await db.SaveChangesAsync();

                TempData["message"] = "Animalul a fost șters cu succes";
            }
            catch (Exception ex)
            {
                TempData["message"] = "A apărut o eroare la ștergerea animalului: " + ex.Message;
            }

            return RedirectToAction("Index", "Animals");
        }

        public async Task<IActionResult> DeleteAnimalTotal(int id)
        {

            var animal = await db.Animals.FindAsync(id);
            if (animal == null)
            {
                TempData["message"] = "Animalul nu a fost găsit";
                return RedirectToAction("Index", "Animals");
            }

            var orders = await db.Comandas
                .Where(c => c.idAnimal == id)
                .ToListAsync();

            if (orders.Any())
            {
                db.Comandas.RemoveRange(orders);
            }

            try
            {
                db.Animals.Remove(animal);
                await db.SaveChangesAsync();

                TempData["message"] = "Animalul a fost adoptat cu succes";
            }
            catch (Exception ex)
            {
                TempData["message"] = "A apărut o eroare la ștergerea animalului: " + ex.Message;
            }

            return RedirectToAction("Index", "Animals");
        }

        public async Task<IActionResult> ShowOrders()
        {
            var orders = await db.Comandas
                .Include(c => c.Animal)
                .Include(c => c.Client)
                .ThenInclude(cl => cl.ApplicationUser) 
                .OrderByDescending(c => c.data_comenzii) 
                .ToListAsync();

            var acceptedOrders = orders
                .Where(c => c.stare == "Finalizată")
                .ToList();

            var pendingOrders = orders
                .Where(c => c.stare == "În așteptare")
                .ToList();

            ViewData["AcceptedOrders"] = acceptedOrders;
            ViewData["PendingOrders"] = pendingOrders;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptOrder(int id)
        {
            var order = await db.Comandas.FindAsync(id);
            if (order == null)
            {
                TempData["message"] = "Comanda nu a fost găsită";
                TempData["messageType"] = "error";
                return RedirectToAction("ShowOrders");
            }

            order.stare = "Finalizată";
            db.Update(order);
            await db.SaveChangesAsync();

            TempData["message"] = "Comanda a fost acceptată cu succes";
            TempData["messageType"] = "success";
            return RedirectToAction("ShowOrders");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectOrder(int id)
        {
            var order = await db.Comandas.FindAsync(id);
            if (order == null)
            {
                TempData["message"] = "Comanda nu a fost găsită";
                TempData["messageType"] = "error";
                return RedirectToAction("ShowOrders");
            }

            order.stare = "Respinsă";
            db.Update(order);
            await db.SaveChangesAsync();

            TempData["message"] = "Comanda a fost respinsă";
            TempData["messageType"] = "warning";
            return RedirectToAction("ShowOrders");
        }

        public IActionResult AddPostare()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPostare(Postare postare)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                postare.ApplicationUserId = user.Id;
                // Set the post date to now
                postare.data_postarii = DateTime.Now;

                db.Add(postare);
                await db.SaveChangesAsync();
                TempData["message"] = "Postarea a fost adăugată cu succes";
                return RedirectToAction("Index", "Postares");
            }
            catch (Exception)
            {
                TempData["message"] = "A apărut o eroare la adăugarea postării";
                TempData["messageType"] = "error";
            }
            return RedirectToAction("Index", "Postares");
        }

        public async Task<IActionResult> DeletePostare(int id)
        {
            try
            {
                var postare = await db.Postares.FindAsync(id);
                var comentarii = db.Comentarius.Where( c => c.idPostare == id);
               
                if (postare == null)
                {
                    TempData["message"] = "Postarea nu a fost găsită";
                    TempData["messageType"] = "error";
                    return RedirectToAction("Index", "Postares");
                }
                if (comentarii.Any())
                {
                    db.Comentarius.RemoveRange(comentarii);
                    await db.SaveChangesAsync();
                }

                db.Postares.Remove(postare);
                await db.SaveChangesAsync();

                TempData["message"] = "Postarea a fost ștearsă cu succes";
                TempData["messageType"] = "success";
            }
            catch (Exception)
            {
                TempData["message"] = "A apărut o eroare la ștergerea postării";
                TempData["messageType"] = "error";
            }

            return RedirectToAction("Index", "Postares");
        }

        public async Task<IActionResult> EditPostare(int id)
        {
            var postare = await db.Postares.FindAsync(id);
            if (postare == null)
            {
                TempData["message"] = "Postarea nu a fost găsită";
                TempData["messageType"] = "error";
                return RedirectToAction("Index", "Postares");
            }
            return View(postare);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPostare(int id, Postare postare)
        {
            if (id != postare.idPostare)
            {
                TempData["message"] = "ID-ul postării nu se potrivește";
                TempData["messageType"] = "error";
                return RedirectToAction("Index", "Postares");
            }
            try
            {
                var existingPostare = await db.Postares.FindAsync(id);
                if (existingPostare == null)
                {
                    TempData["message"] = "Postarea nu a fost găsită";
                    TempData["messageType"] = "error";
                    return RedirectToAction("Index", "Postares");
                }

                existingPostare.titlu = postare.titlu;
                existingPostare.descriere = postare.descriere;

                db.Update(existingPostare);
                await db.SaveChangesAsync();

                TempData["message"] = "Postarea a fost actualizată cu succes";
                TempData["messageType"] = "success";
                return RedirectToAction("Index", "Postares");
            }
            catch (Exception)
            {
                TempData["message"] = "A apărut o eroare la actualizarea postării";
                TempData["messageType"] = "error";
            }

            return View(postare);
        }
    }
}
