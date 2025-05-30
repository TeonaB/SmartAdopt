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

        public AdminsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, IWebHostEnvironment env)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
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
                var comentarii = db.Comentarius.Where(c => c.idPostare == id);

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

        public async Task<IActionResult> ShowAll()
        {
            var users = db.Users.OrderBy(u => u.nume);

            var usersWithRoles = new List<(ApplicationUser User, string Role)>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? "Client";
                usersWithRoles.Add((user, role));
            }

            var sortedUsers = usersWithRoles
                .OrderBy(ur => ur.Role == "Admin" ? 0 : 1)
                .ThenBy(ur => ur.User.nume);

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }


            ViewBag.UsersList = sortedUsers;
            return View();
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    TempData["message"] = "Utilizatorul nu a fost găsit.";
                    TempData["messageType"] = "error";
                    return RedirectToAction("ShowAll", "Admins");
                }

                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault();

                if (role == "Admin")
                {
                    var admin = await db.Admins
                        .FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

                    var postari = await db.Postares
                        .Where(p => p.ApplicationUserId == user.Id)
                        .ToListAsync();

                    if (postari.Any())
                    {
                        foreach (var postare in postari)
                        {
                            var comentarii = db.Comentarius.Where(c => c.idPostare == postare.idPostare);
                            if (comentarii.Any())
                            {
                                db.Comentarius.RemoveRange(comentarii);
                                await db.SaveChangesAsync();
                            }

                            db.Postares.Remove(postare);
                            await db.SaveChangesAsync();
                        }
                    }

                    db.Admins.Remove(admin);
                    await db.SaveChangesAsync();
                }
                else if (role == "Client")
                {
                    var client = await db.Clients
                        .FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

                    if (client != null)
                    {

                        var comenzi = await db.Comandas
                            .Where(c => c.idClient == client.idClient)
                            .ToListAsync();

                        if (comenzi.Any())
                        {
                            db.Comandas.RemoveRange(comenzi);
                            await db.SaveChangesAsync();
                        }

                        var comentarii = await db.Comentarius
                            .Where(c => c.idClient == client.idClient)
                            .ToListAsync();

                        if (comentarii.Any())
                        {
                            db.Comentarius.RemoveRange(comentarii);
                            await db.SaveChangesAsync();
                        }

                        var raspChestionars = await db.RaspChestionars
                        .Where(rc => rc.idClient == client.idClient)
                        .Include(rc => rc.RaspAnimals) 
                        .ToListAsync();

                        if (raspChestionars.Any())
                        {
                            foreach (var raspChestionar in raspChestionars)
                            {
                                if (raspChestionar.RaspAnimals != null)
                                {
                                    db.RaspAnimals.RemoveRange(raspChestionar.RaspAnimals);
                                }
                                db.RaspChestionars.Remove(raspChestionar);
                            }
                            await db.SaveChangesAsync();
                        }
                        db.Clients.Remove(client);
                        await db.SaveChangesAsync();
                    }
                }

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    TempData["message"] = "Eroare la ștergerea utilizatorului: " + string.Join(", ", result.Errors.Select(e => e.Description));
                    TempData["messageType"] = "error";
                    return RedirectToAction("ShowAll", "Admins");
                }

                TempData["message"] = "Utilizatorul a fost șters cu succes.";
                TempData["messageType"] = "success";
            }
            catch (Exception ex)
            {
                TempData["message"] = "A apărut o eroare: " + ex.Message;
                TempData["messageType"] = "error";
            }

            return RedirectToAction("ShowAll", "Admins");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SchimbareRol(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    TempData["message"] = "Utilizatorul nu a fost găsit.";
                    TempData["messageType"] = "error";
                    return RedirectToAction("ShowAll", "Admins");
                }

                var roles = await _userManager.GetRolesAsync(user);
                var currentRole = roles.FirstOrDefault();


                string newRole = "";
                if (currentRole == "Admin")
                {
                    newRole = "Client";
                }
                else if (currentRole == "Client")
                {
                    newRole = "Admin";
                }

                if (currentRole =="Client")
                {
                    var client = await db.Clients
                       .FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

                    if (client != null)
                    {
                        var comenzi = await db.Comandas
                            .Where(c => c.idClient == client.idClient)
                            .ToListAsync();

                        if (comenzi.Any())
                        {
                            db.Comandas.RemoveRange(comenzi);
                            await db.SaveChangesAsync();
                        }

                        var comentarii = await db.Comentarius
                            .Where(c => c.idClient == client.idClient)
                            .ToListAsync();

                        if (comentarii.Any())
                        {
                            db.Comentarius.RemoveRange(comentarii);
                            await db.SaveChangesAsync();
                        }

                        var raspChestionars = await db.RaspChestionars
                        .Where(rc => rc.idClient == client.idClient)
                        .Include(rc => rc.RaspAnimals)
                        .ToListAsync();

                        if (raspChestionars.Any())
                        {
                            foreach (var raspChestionar in raspChestionars)
                            {
                                if (raspChestionar.RaspAnimals != null)
                                {
                                    db.RaspAnimals.RemoveRange(raspChestionar.RaspAnimals);
                                }
                                db.RaspChestionars.Remove(raspChestionar);
                            }
                            await db.SaveChangesAsync();
                        }
                        db.Clients.Remove(client);
                        await db.SaveChangesAsync();
                    }

                    var admin = new Admin
                    {

                        ApplicationUserId = user.Id
                    };
                    db.Admins.Add(admin);
                    db.SaveChanges();
                }
                else
                {
                    var admin = await db.Admins
                        .FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

                    var postari = await db.Postares
                        .Where(p => p.ApplicationUserId == user.Id)
                        .ToListAsync();

                    if (postari.Any())
                    {
                        foreach (var postare in postari)
                        {
                            var comentarii = db.Comentarius.Where(c => c.idPostare == postare.idPostare);
                            if (comentarii.Any())
                            {
                                db.Comentarius.RemoveRange(comentarii);
                                await db.SaveChangesAsync();
                            }

                            db.Postares.Remove(postare);
                            await db.SaveChangesAsync();
                        }
                    }

                    db.Admins.Remove(admin);
                    await db.SaveChangesAsync();

                    var client = new Client
                    {

                        ApplicationUserId = user.Id,
                        idRaspChestionar = 0,
                        CompletedProfile = false,
                        nr_telefon = " ",
                        adresa = " "
                    };
                    db.Clients.Add(client);
                    db.SaveChanges();
                }
                if (currentRole != null)
                {
                    var removeResult = await _userManager.RemoveFromRoleAsync(user, currentRole);
                    if (!removeResult.Succeeded)
                    {
                        TempData["message"] = "Eroare la eliminarea rolului curent: " + string.Join(", ", removeResult.Errors.Select(e => e.Description));
                        TempData["messageType"] = "error";
                        return RedirectToAction("ShowAll", "Admins");
                    }
                }

                var addResult = await _userManager.AddToRoleAsync(user, newRole);
                if (!addResult.Succeeded)
                {
                    TempData["message"] = "Eroare la asignarea noului rol: " + string.Join(", ", addResult.Errors.Select(e => e.Description));
                    TempData["messageType"] = "error";
                    return RedirectToAction("ShowAll", "Admins");
                }
   
            }
            catch (Exception ex)
            {
                TempData["message"] = "A apărut o eroare: " + ex.Message;
                TempData["messageType"] = "error";
                return RedirectToAction("ShowAll", "Admins");
            }

            var user2 = await _userManager.GetUserAsync(User);
            if(user2.Id == id)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Clients");
            }
            else
            {
                TempData["message"] = "Rolul utilizatorului a fost schimbat cu succes";
                TempData["messageType"] = "success";
                return RedirectToAction("ShowAll", "Admins");
            } 
        }

        public async Task<IActionResult> ViewClient(string id)
        {
            try
            {
                var client = await db.Clients
                    .Include(c => c.ApplicationUser)
                    .Include(c => c.Comandas)
                    .FirstOrDefaultAsync(c => c.ApplicationUserId == id);

                if (client == null)
                {
                    TempData["message"] = "Clientul nu a fost găsit.";
                    TempData["messageType"] = "error";
                    return RedirectToAction("ShowAll", "Admins");
                }
                var orders = await db.Comandas
               .Where(c => c.idClient == client.idClient)
               .Include(c => c.Animal)
               .ToListAsync();
                ViewBag.orders = orders;
                return View(client);
            }
            catch (Exception ex)
            {
                TempData["message"] = "A apărut o eroare: " + ex.Message;
                TempData["messageType"] = "error";
                return RedirectToAction("ShowAll", "Admins");
            }
        }
    }
}
