using DinkToPdf.Contracts;
using DinkToPdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
        private readonly ICompositeViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private IWebHostEnvironment _env;

        public AdminsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, IWebHostEnvironment env, ICompositeViewEngine viewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _env = env;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Index", "Home");
            var admin = await db.Admins.FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            ViewBag.AnimalCount = await db.Animals.CountAsync();
            ViewBag.ComenziCount = await db.Comandas.Where(c => c.stare != "Respinsa").CountAsync();

            return View(user);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AddAnimal()
        {
            return View(new Animal());
        }


        private string databaseFileName;
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAnimal(Animal animal, IFormFile? imageFile)
        {

            if (imageFile != null && imageFile.Length > 0)
            {
                // Generam calea de stocare a fisierului
                var storagePath = Path.Combine(
                _env.WebRootPath, 
                "images/animals", 
                imageFile.FileName 
                );

                // Generam calea de afisare a fisierului
                databaseFileName = "/images/animals/" + imageFile.FileName;

                // Uploadam fisierul la cale
                using (var fileStream = new FileStream(storagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }
            }
            else
            {
                databaseFileName = "/images/logo.png";
            }

            animal.ImagePath = databaseFileName;
            db.Animals.Add(animal);
            await db.SaveChangesAsync();
            TempData["message"] = "Animalutul a fost adaugat";
            return RedirectToAction("Index", "Animals");

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditAnimal(int id)
        {
            var animal = await db.Animals.FindAsync(id);
            if (animal == null)
            {
                TempData["message"] = "Animalul nu a fost gasit";
                TempData["messageType"] = "error";
                return RedirectToAction("Index", "Animals");
            }

            return View(animal);
        }

        [Authorize(Roles = "Admin")]
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
                TempData["message"] = "Animalul nu a fost gasit";
                TempData["messageType"] = "error";
                return RedirectToAction("Index", "Animals");
            }

            if (ImagePath != null && ImagePath.Length > 0)
            {
                // Generam calea de stocare a fisierului
                var storagePath = Path.Combine(
                _env.WebRootPath,
                "images/animals",
                ImagePath.FileName
                );

                // Generam calea de afisare a fisierului
                databaseFileName = "/images/animals/" + ImagePath.FileName;

                // Uploadam fisierul la cale
                using (var fileStream = new FileStream(storagePath, FileMode.Create))
                {
                    await ImagePath.CopyToAsync(fileStream);
                }

                animal.ImagePath = databaseFileName;
            }
            else
            {
                animal.ImagePath = existingAnimal.ImagePath;
            }

            try
            {
                db.Update(animal);
                await db.SaveChangesAsync();
                TempData["message"] = "Animalul a fost actualizat";
                TempData["messageType"] = "success";
            }
            catch (DbUpdateException)
            {
                TempData["message"] = "A aparut o eroare la actualizarea animalului";
                TempData["messageType"] = "error";
            }
            return RedirectToAction("Index", "Animals");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAnimal(int id)
        {

            var animal = await db.Animals.FindAsync(id);
            if (animal == null)
            {
                TempData["message"] = "Animalul nu a fost gasit";
                return RedirectToAction("Index", "Animals");
            }

            var orders = await db.Comandas.Where(c => c.idAnimal == id).ToListAsync();
            if (orders.Any())
            {
                var areComenzi = orders.Any(c => c.stare != "Respinsa");

                if (areComenzi)
                {
                    TempData["message"] = "Animalul nu poate fi sters deoarece are comenzi asociate care nu sunt respinse";
                    TempData["messageType"] = "warning";
                    return RedirectToAction("Index", "Animals");
                }
                else
                {
                    db.Comandas.RemoveRange(orders);
                }
            }

            var chestionar = await db.RaspAnimals.Where( r => r.idAnimal == id).ToListAsync();
            if(chestionar.Any())
            {
                db.RaspAnimals.RemoveRange(chestionar);
            }
            try
            {
                db.Animals.Remove(animal);
                await db.SaveChangesAsync();

                TempData["message"] = "Animalul a fost sters";
            }
            catch (Exception ex)
            {
                TempData["message"] = "A aparut o eroare la stergerea animalului: " + ex.Message;
            }

            return RedirectToAction("Index", "Animals");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAnimalTotal(int id)
        {

            var animal = await db.Animals.FindAsync(id);
            if (animal == null)
            {
                TempData["message"] = "Animalul nu a fost gasit";
                return RedirectToAction("Index", "Animals");
            }

            var orders = await db.Comandas.Where(c => c.idAnimal == id).ToListAsync();
            if (orders.Any())
            {
                db.Comandas.RemoveRange(orders);
            }

            var chestionar = await db.RaspAnimals.Where(r => r.idAnimal == id).ToListAsync();
            if (chestionar.Any())
            {
                db.RaspAnimals.RemoveRange(chestionar);
            }

            try
            {
                db.Animals.Remove(animal);
                var animalsAdoptat = db.AnimalAdoptats.FirstOrDefault(c => c.idAnimalAdoptat == 1);
                animalsAdoptat.counter++;
                await db.SaveChangesAsync();
                
                TempData["message"] = "Animalul a fost adoptat";
            }
            catch (Exception ex)
            {
                TempData["message"] = "A aparut o eroare la stergerea animalului: " + ex.Message;
            }

            return RedirectToAction("Index", "Animals");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ShowComenzi()
        {
            var comenzi = await db.Comandas
                .Include(c => c.Animal)
                .Include(c => c.Client)
                .ThenInclude(cl => cl.ApplicationUser)
                .OrderByDescending(c => c.data_comenzii)
                .ToListAsync();
            var comenziacceptate = comenzi.Where(c => c.stare == "Finalizata").ToList();
            var comenzipending = comenzi.Where(c => c.stare == "In asteptare").ToList();

            ViewData["comenziacceptate"] = comenziacceptate;
            ViewData["comenzipending"] = comenzipending;

            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptComanda(int id)
        {
            var comanda = await db.Comandas.FindAsync(id);
            if (comanda == null)
            {
                TempData["message"] = "Comanda nu a fost gasita";
                TempData["messageType"] = "error";
                return RedirectToAction("ShowComenzi");
            }

            comanda.stare = "Finalizata";
            db.Update(comanda);
            await db.SaveChangesAsync();

            TempData["message"] = "Comanda a fost acceptata";
            TempData["messageType"] = "success";
            return RedirectToAction("ShowComenzi");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectComanda(int id)
        {
            var comanda = await db.Comandas.FindAsync(id);
            if (comanda == null)
            {
                TempData["message"] = "Comanda nu a fost gasita";
                TempData["messageType"] = "error";
                return RedirectToAction("ShowComenzi");
            }

            comanda.stare = "Respinsa";
            db.Update(comanda);
            await db.SaveChangesAsync();

            TempData["message"] = "Comanda a fost respinsa";
            TempData["messageType"] = "warning";
            return RedirectToAction("ShowComenzi");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AddPostare()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPostare(Postare postare)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                postare.ApplicationUserId = user.Id;
                postare.data_postarii = DateTime.Now;

                db.Add(postare);
                await db.SaveChangesAsync();
                TempData["message"] = "Postarea a fost adaugata";
                return RedirectToAction("Index", "Postares");
            }
            catch (Exception)
            {
                TempData["message"] = "A aparut o eroare la adaugarea postarii";
                TempData["messageType"] = "error";
            }
            return RedirectToAction("Index", "Postares");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePostare(int id)
        {
            try
            {
                var postare = await db.Postares.FindAsync(id);
                var comentarii = db.Comentarius.Where(c => c.idPostare == id);

                if (postare == null)
                {
                    TempData["message"] = "Postarea nu a fost gasita";
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

                TempData["message"] = "Postarea a fost stearsa";
                TempData["messageType"] = "success";
            }
            catch (Exception)
            {
                TempData["message"] = "A aparut o eroare la stergerea postarii";
                TempData["messageType"] = "error";
            }

            return RedirectToAction("Index", "Postares");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditPostare(int id)
        {
            var postare = await db.Postares.FindAsync(id);
            if (postare == null)
            {
                TempData["message"] = "Postarea nu a fost gasita";
                TempData["messageType"] = "error";
                return RedirectToAction("Index", "Postares");
            }
            return View(postare);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPostare(int id, Postare postare)
        {
            if (id != postare.idPostare)
            {
                TempData["message"] = "ID-ul postarii nu se potriveste";
                TempData["messageType"] = "error";
                return RedirectToAction("Index", "Postares");
            }
            try
            {
                var existingPostare = await db.Postares.FindAsync(id);
                if (existingPostare == null)
                {
                    TempData["message"] = "Postarea nu a fost gasita";
                    TempData["messageType"] = "error";
                    return RedirectToAction("Index", "Postares");
                }

                existingPostare.titlu = postare.titlu;
                existingPostare.descriere = postare.descriere;

                db.Update(existingPostare);
                await db.SaveChangesAsync();

                TempData["message"] = "Postarea a fost actualizata";
                TempData["messageType"] = "success";
                return RedirectToAction("Index", "Postares");
            }
            catch (Exception)
            {
                TempData["message"] = "A aparut o eroare la actualizarea postarii";
                TempData["messageType"] = "error";
            }

            return View(postare);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ShowUseri()
        {
            var users = db.Users.OrderBy(u => u.nume);

            var usersWithRoles = new List<(ApplicationUser User, string Role)>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? "Client";
                usersWithRoles.Add((user, role));
            }
            var sortedUsers = usersWithRoles.OrderBy(ur => ur.Role == "Admin" ? 0 : 1).ThenBy(ur => ur.User.nume);

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            ViewBag.UsersList = sortedUsers;
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditClient(int id)
        {
            var client = await db.Clients
                .Include(c => c.ApplicationUser)
                .FirstOrDefaultAsync(c => c.idClient == id);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditClient(int id, Client client)
        {
            if (id != client.idClient)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingClient = await db.Clients
                        .Include(c => c.ApplicationUser)
                        .FirstOrDefaultAsync(c => c.idClient == id);

                    if (existingClient == null)
                    {
                        return NotFound();
                    }

                    existingClient.nr_telefon = client.nr_telefon;
                    existingClient.adresa = client.adresa;

                    existingClient.ApplicationUser.nume = client.ApplicationUser.nume;
                    existingClient.ApplicationUser.prenume = client.ApplicationUser.prenume;

                    db.Update(existingClient);
                    await db.SaveChangesAsync();

                    TempData["message"] = "Clientul a fost actualizat cu succes!";
                    TempData["messageType"] = "success";
                    return RedirectToAction("ViewClient", new { id = client.ApplicationUserId });
                }
                catch
                {
                    TempData["message"] = "A apărut o eroare la actualizarea clientului.";
                    TempData["messageType"] = "error";
                }
            }
            return View(client);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    TempData["message"] = "Utilizatorul nu a fost gasit.";
                    TempData["messageType"] = "error";
                    return RedirectToAction("ShowUseri", "Admins");
                }

                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault();

                if (role == "Admin")
                {
                    var admin = await db.Admins.FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

                    var postari = await db.Postares.Where(p => p.ApplicationUserId == user.Id).ToListAsync();
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

                        var comenzi = await db.Comandas.Where(c => c.idClient == client.idClient).ToListAsync();
                        if (comenzi.Any())
                        {
                            db.Comandas.RemoveRange(comenzi);
                            await db.SaveChangesAsync();
                        }

                        var comentarii = await db.Comentarius.Where(c => c.idClient == client.idClient).ToListAsync();
                        if (comentarii.Any())
                        {
                            db.Comentarius.RemoveRange(comentarii);
                            await db.SaveChangesAsync();
                        }

                        var raspChestionars = await db.RaspChestionars.Where(rc => rc.idClient == client.idClient).Include(rc => rc.RaspAnimals).ToListAsync();
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
                    TempData["message"] = "Eroare la stergerea utilizatorului: " + string.Join(", ", result.Errors.Select(e => e.Description));
                    TempData["messageType"] = "error";
                    return RedirectToAction("ShowUseri", "Admins");
                }

                TempData["message"] = "Utilizatorul a fost sters";
                TempData["messageType"] = "success";
            }
            catch (Exception ex)
            {
                TempData["message"] = "A aparut o eroare: " + ex.Message;
                TempData["messageType"] = "error";
            }

            return RedirectToAction("ShowUseri", "Admins");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SchimbareRol(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    TempData["message"] = "Utilizatorul nu a fost gasit.";
                    TempData["messageType"] = "error";
                    return RedirectToAction("ShowUseri", "Admins");
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
                    var client = await db.Clients.FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

                    if (client != null)
                    {
                        var comenzi = await db.Comandas.Where(c => c.idClient == client.idClient).ToListAsync();
                        if (comenzi.Any())
                        {
                            db.Comandas.RemoveRange(comenzi);
                            await db.SaveChangesAsync();
                        }

                        var comentarii = await db.Comentarius.Where(c => c.idClient == client.idClient).ToListAsync();
                        if (comentarii.Any())
                        {
                            db.Comentarius.RemoveRange(comentarii);
                            await db.SaveChangesAsync();
                        }

                        var raspChestionars = await db.RaspChestionars.Where(rc => rc.idClient == client.idClient).Include(rc => rc.RaspAnimals).ToListAsync();
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
                    var admin = await db.Admins.FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

                    var postari = await db.Postares.Where(p => p.ApplicationUserId == user.Id).ToListAsync();
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
                        nr_telefon = "",
                        adresa = ""
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
                        return RedirectToAction("ShowUseri", "Admins");
                    }
                }

                var addResult = await _userManager.AddToRoleAsync(user, newRole);
                if (!addResult.Succeeded)
                {
                    TempData["message"] = "Eroare la asignarea noului rol: " + string.Join(", ", addResult.Errors.Select(e => e.Description));
                    TempData["messageType"] = "error";
                    return RedirectToAction("ShowUseri", "Admins");
                }
   
            }
            catch (Exception ex)
            {
                TempData["message"] = "A aparut o eroare: " + ex.Message;
                TempData["messageType"] = "error";
                return RedirectToAction("ShowUseri", "Admins");
            }

            var user2 = await _userManager.GetUserAsync(User);
            if(user2.Id == id)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Clients");
            }
            else
            {
                TempData["message"] = "Rolul utilizatorului a fost schimbat";
                TempData["messageType"] = "success";
                return RedirectToAction("ShowUseri", "Admins");
            } 
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ViewClient(string id)
        {
            try
            {
                var client = await db.Clients.Include(c => c.ApplicationUser).Include(c => c.Comandas).FirstOrDefaultAsync(c => c.ApplicationUserId == id);

                if (client == null)
                {
                    TempData["message"] = "Clientul nu a fost gasit.";
                    TempData["messageType"] = "error";
                    return RedirectToAction("ShowUseri", "Admins");
                }
                var comenzi = await db.Comandas.Where(c => c.idClient == client.idClient).Include(c => c.Animal).ToListAsync();
                ViewBag.comenzi = comenzi;
                return View(client);
            }
            catch (Exception ex)
            {
                TempData["message"] = "A aparut o eroare: " + ex.Message;
                TempData["messageType"] = "error";
                return RedirectToAction("ShowUseri", "Admins");
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Raport()
        {
            ViewBag.IsPdf = false;
            var viewModel = await GetRaport();
            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        public async Task<RaportStatisticiViewModel> GetRaport()
        {
            // Pentru utilizatori
            var totalUtilizatoriInregistrati = await db.Clients.CountAsync();
            var utilizatoriCuProfilComplet = await db.Clients.CountAsync(c => c.CompletedProfile == true);
            var totiIdUtilizatori = await db.Clients.Select(c => c.idClient).ToListAsync();
            var utilizatoriCuComentarii = await db.Comentarius.Select(c => c.idClient).Distinct().ToListAsync();
            var utilizatoriInteractiuniBlog = utilizatoriCuComentarii.Count;

            // Pentru recomandari
            var totalRecomandariGenerat = await db.RaspChestionars.CountAsync();
            var topAnimaleRecomandate = await db.RaspAnimals
                .GroupBy(ra => ra.idAnimal)
                .Select(g => new { IdAnimal = g.Key, Numar = g.Count() })
                .OrderByDescending(g => g.Numar)
                .Take(5)
                .Join(db.Animals, g => g.IdAnimal, a => a.idAnimal, (g, a) => new { a.nume, a.specie, Numar = g.Numar })
                .ToListAsync();
            var comenziDinRecomandari = await db.Comandas
                 .Join(db.Clients,
                     comanda => comanda.idClient,
                     client => client.idClient,
                     (comanda, client) => new { comanda, client })
                 .Where(x => x.client.idRaspChestionar != 0)
                 .Join(db.RaspChestionars,
                     x => x.client.idRaspChestionar,
                     rc => rc.idRasp,
                     (x, rc) => new { x.comanda, x.client, rc })
                 .Join(db.RaspAnimals,
                     x => new { x.rc.idRasp, idAnimal = x.comanda.idAnimal },
                     ra => new { ra.idRasp, ra.idAnimal },
                     (x, ra) => new { x.comanda, x.client, x.rc, ra })
                 .CountAsync();

            // Pentru animale
            var totalAnimale = await db.Animals.CountAsync();
            var animalePeSpecie = await db.Animals
                .GroupBy(a => a.specie)
                .Select(g => new { Specie = g.Key, Numar = g.Count() })
                .ToListAsync();
            var atributeMedii = new
            {
                MarimeMedie = await db.Animals.AverageAsync(a => (double)a.marime),
                EnergieMedie = await db.Animals.AverageAsync(a => (double)a.nivel_energie),
                AtentieMedie = await db.Animals.AverageAsync(a => (double)a.nivel_atentie_necesara),
                AdaptabilitateMedie = await db.Animals.AverageAsync(a => (double)a.nivel_adaptabilitate),
                GrupaVarstaMedie = await db.Animals.AverageAsync(a => (double)a.grupa_varsta)
            };
            var animaleNerecomandate = await db.Animals
                .GroupJoin(db.RaspAnimals, a => a.idAnimal, ra => ra.idAnimal, (a, ra) => new { Animal = a, AreRecomandare = ra.Any() })
                .Where(x => !x.AreRecomandare)
                .Select(x => x.Animal)
                .ToListAsync();
            var animalAdoptat = db.AnimalAdoptats.FirstOrDefault(c => c.idAnimalAdoptat == 1);
            var totalAnimaleAdoptate = animalAdoptat.counter;
            var statusuriComenzi = await db.Comandas
                .GroupBy(c => c.stare)
                .Select(g => new { Stare = g.Key, Numar = g.Count() })
                .ToListAsync();

            return new RaportStatisticiViewModel
            {
                TotalUseri = totalUtilizatoriInregistrati,
                UseriCuProfilCompletat = utilizatoriCuProfilComplet,
                UseriCuBlog = utilizatoriInteractiuniBlog,
                TotalRecomandari = totalRecomandariGenerat,
                TopAnimaleRecomandate = topAnimaleRecomandate.Cast<dynamic>().ToList(),
                ComenziDinRecomandari = comenziDinRecomandari,
                TotalAnimale = totalAnimale,
                AnimalePeSpecie = animalePeSpecie.Cast<dynamic>().ToList(),
                AverageAtribute = atributeMedii,
                AnimaleNerecomandate = animaleNerecomandate,
                TotalAnimaleAdoptate = totalAnimaleAdoptate,
                StatusComenzi = statusuriComenzi.Cast<dynamic>().ToList()
            };

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DownloadRaportAsPdf()
        {
            var viewModel = await GetRaport();
            ViewBag.IsPdf = true;
            // Transform html in string
            var htmlContent = await RenderViewToStringAsync("Raport", viewModel);

            // Configurez setarile pentru pdf
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4
            };
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = htmlContent,
                WebSettings =
                {
                    DefaultEncoding = "utf-8",
                    EnableJavascript = true,
                    LoadImages = true
                }
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            // Convertim Html in Pdf
            var converter = _serviceProvider.GetRequiredService<IConverter>();
            var pdfBytes = converter.Convert(pdf);

            return File(pdfBytes, "application/pdf", "Raport.pdf");
        }


        private async Task<string> RenderViewToStringAsync(string viewName, object model)
        {
            var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);
            using var writer = new StringWriter();
            var viewContext = new ViewContext(
            ControllerContext,
                viewResult.View,
                new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = model },
                new TempDataDictionary(HttpContext, _tempDataProvider),
                writer,
                new HtmlHelperOptions()
            );
            await viewResult.View.RenderAsync(viewContext);
            return writer.ToString();
        }

    }
}
