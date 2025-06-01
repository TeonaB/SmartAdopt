using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartAdopt.Data;
using SmartAdopt.Models;
using System.Linq;

namespace SmartAdopt.Controllers
{
    [Authorize(Roles = "Client")]
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

        public async Task<IActionResult> EditNr()
        {
            var userId = _userManager.GetUserId(User);
            var client = await db.Clients.FirstOrDefaultAsync(s => s.ApplicationUserId == userId);
            if (string.IsNullOrEmpty(userId))
            {
                return new ChallengeResult(); 
            }
            if (client == null)
            {
                return NotFound();
            }
            if (TempData.ContainsKey("ErrorMessage"))
            {
                ViewBag.Message = TempData["ErrorMessage"];
                return View(client);
            }
            return View(client);
        }

        [HttpPost]
        public IActionResult EditNr(int id, Client client)
        {
            if (ModelState.IsValid)
            {
                return View(client);
            }
            else
            {
                Client newclient = db.Clients.Where(stu => stu.idClient == id).First();

                if (newclient == null)
                {
                    return NotFound();
                }

                newclient.nr_telefon = client.nr_telefon;
                if (!string.IsNullOrEmpty(client.nr_telefon) && !string.IsNullOrEmpty(client.adresa))
                {
                    newclient.CompletedProfile = true;
                    db.SaveChanges();
                }
                if (!IsValidPhoneNumber(client.nr_telefon))
                {
                    TempData["ErrorMessage"] = "Ati introdus un nr de telefon gresit";
                    return RedirectToAction("EditNr", "Clients");
                }
                bool IsValidPhoneNumber(string phoneNumber)
                {
                    if (string.IsNullOrWhiteSpace(phoneNumber))
                        return false;

                    return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^07\d{8}$");
                }
                db.SaveChanges();
                TempData["SuccessMessage"] = "Updated successfully.";
                return RedirectToAction("Index", "Clients");
            }
        }

        public async Task<IActionResult> EditAdresa()
        {
            var userId = _userManager.GetUserId(User);
            var client = await db.Clients.FirstOrDefaultAsync(s => s.ApplicationUserId == userId);
            if (string.IsNullOrEmpty(userId))
            {
                return new ChallengeResult();
            }
            if (client == null)
            {
                return NotFound();
            }
            if (TempData.ContainsKey("ErrorMessage"))
            {
                ViewBag.Message = TempData["ErrorMessage"];
                return View(client);
            }
            return View(client);
        }

        [HttpPost]
        public IActionResult EditAdresa(int id, Client client)
        {
            if (ModelState.IsValid)
            {
                return View(client);
            }
            else
            {
                Client newclient = db.Clients.Where(stu => stu.idClient == id).First();

                if (newclient == null)
                {
                    return NotFound();
                }

                newclient.adresa = client.adresa;
                if (!string.IsNullOrEmpty(client.nr_telefon) && !string.IsNullOrEmpty(client.adresa))
                {
                    newclient.CompletedProfile = true;
                    db.SaveChanges();
                }
                if (string.IsNullOrWhiteSpace(client.adresa))  
                {
                    TempData["ErrorMessage"] = "Ati introdus o adresa gresita";
                    return RedirectToAction("EditAdresa", "Students");
                }
                db.SaveChanges();
                TempData["SuccessMessage"] = "Updated successfully.";
                return RedirectToAction("Index", "Clients");
            }
        }

        public async Task<IActionResult> CreateOrder(int id)
        {
            var animal = await db.Animals.FindAsync(id);
            if (animal == null)
            {
                return RedirectToAction("Index", "Animals");
            }
            var userId = _userManager.GetUserId(User);
            var client = await db.Clients.FirstOrDefaultAsync(s => s.ApplicationUserId == userId);
            if (client == null)
            {
                return Unauthorized();
            }

            var comanda = new Comanda
            {
                idAnimal = id,
                idClient = client.idClient, 
                data_comenzii = DateTime.Now,
                stare = "În așteptare", 
                total_plata = animal.pret, 
                metoda_platii = "", 
                motivatie = "" 
            };
            ViewBag.nume = animal.nume;
            return View(comanda);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrder(Comanda comanda)
        {

            var userId = _userManager.GetUserId(User);
            var client = await db.Clients.FirstOrDefaultAsync(s => s.ApplicationUserId == userId);
            if (client == null)
            {
                return Unauthorized();
            }
            comanda.idClient = client.idClient; 
            comanda.data_comenzii = DateTime.Now;
            comanda.stare = "În așteptare";
            db.Comandas.Add(comanda);
            await db.SaveChangesAsync();

            TempData["message"] = "Comanda a fost creată cu succes";
            return RedirectToAction("Index", "Animals");
        }

        public async Task<IActionResult> ShowOrders()
        {
            var userId = _userManager.GetUserId(User);
            var client = await db.Clients.FirstOrDefaultAsync(s => s.ApplicationUserId == userId);
            if (client == null)
            {
                return Unauthorized();
            }

            int clientId = client.idClient; 

            var orders = await db.Comandas
                .Where(c => c.idClient == clientId)
                .Include(c => c.Animal) 
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Chestionar()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var client = await db.Clients
                .FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

            if (client == null)
            {
                TempData["message"] = "Client not found.";
                return RedirectToAction("Index", "Home");
            }

            if (client.idRaspChestionar == 0)
            {
                ViewBag.Chestionar = 0;
                return View(new ChestionarViewModel());
            }
            else
            {
                client = await db.Clients
                .Include(c => c.RaspChestionar)
                .ThenInclude(rc => rc.RaspAnimals)
                .ThenInclude(ra => ra.Animal)
                .FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

                var recommendedAnimals = await db.RaspChestionars
                    .Where(rc => rc.idRasp == client.idRaspChestionar)
                    .SelectMany(rc => rc.RaspAnimals)
                    .Select(ra => ra.Animal)
                    .Take(5)
                    .ToListAsync();

                ViewBag.Chestionar = 1;
                ViewBag.RecommendedAnimals = recommendedAnimals;
                return View();
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Chestionar(ChestionarViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var client = await db.Clients.FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);
            if (client == null)
            {
                TempData["message"] = "Client not found.";
                return RedirectToAction("Index", "Home");
            }

            var recommendedAnimals = await GetRecommendedAnimalsAsync(model);

            var raspChestionar = new RaspChestionar { idClient = client.idClient };
            db.RaspChestionars.Add(raspChestionar);
            await db.SaveChangesAsync(); 

            client.idRaspChestionar = raspChestionar.idRasp;
            db.Clients.Update(client); 
            await db.SaveChangesAsync(); 

            foreach (var animal in recommendedAnimals)
            {
                db.RaspAnimals.Add(new RaspAnimal
                {
                    idRasp = raspChestionar.idRasp,
                    idAnimal = animal.idAnimal
                });
            }
            await db.SaveChangesAsync(); 

            return RedirectToAction("Chestionar");
        }

        private async Task<List<Animal>> GetRecommendedAnimalsAsync(ChestionarViewModel model)
        {
            var animals = await db.Animals.ToListAsync();

            int CalculateTieredScore(int userValue, int animalValue)
            {
                int diff = Math.Abs(userValue - animalValue);
                return diff switch
                {
                    0 => 10,  
                    1 => 7,   
                    2 => 5,   
                    3 => 4,   
                    4 => 1,   
                    _ => 0    
                };
            }

            var scoredAnimals = animals.Select(a => new
            {
                Animal = a,
                Score = 0 
                          // 1. LivingSituation: Apartment or House
                        + (model.LivingSituation == "Apartment" && a.marime <= 3 ? 10 : 0)
                        + (model.LivingSituation == "House" && a.marime > 3 ? 10 : 0)

                        // 2. HasYard: Energy level
                        + (model.HasYard && a.nivel_energie >= 3 ? 10 : 0)
                        + (!model.HasYard && a.nivel_energie <= 2 ? 10 : 0)

                        // 3. ExerciseTime: Attention required
                        + (model.ExerciseTime == "Less than 30 minutes" && a.nivel_atentie_necesara <= 2 ? 10 : 0)
                        + (model.ExerciseTime == "30 minutes to 1 hour" && a.nivel_atentie_necesara == 3 ? 10 : 0)
                        + (model.ExerciseTime == "More than 1 hour" && a.nivel_atentie_necesara >= 4 ? 10 : 0)

                        // 4. HasOtherPets and HasChildren: Adaptability
                        + (model.HasOtherPets && model.HasChildren && a.nivel_adaptabilitate >= 4 ? 10 : 0)
                        + ((model.HasOtherPets ^ model.HasChildren) && (a.nivel_adaptabilitate == 2 || a.nivel_adaptabilitate == 3) ? 10 : 0)
                        + (!model.HasOtherPets && !model.HasChildren && a.nivel_adaptabilitate == 1 ? 10 : 0)

                        // 5. Numerical preferences with tiered scoring
                        + CalculateTieredScore(model.PreferredSize, a.marime)
                        + CalculateTieredScore(model.AttentionLevel, a.nivel_atentie_necesara)
                        + CalculateTieredScore(model.PreferredAgeGroup, a.grupa_varsta)
                        + CalculateTieredScore(model.AdaptabilityImportance, a.nivel_adaptabilitate)
            })
            .OrderByDescending(x => x.Score)
            .Select(x => x.Animal)
            .Take(5)
            .ToList();

            return scoredAnimals;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetChestionar()
        {
            var user = await _userManager.GetUserAsync(User);
            var client = await db.Clients.FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);
            if (client == null)
            {
                TempData["message"] = "Client not found.";
                return RedirectToAction("Index", "Home");
            }
            var chestionar = await db.RaspChestionars.FirstOrDefaultAsync(c => c.idRasp == client.idRaspChestionar);
            if (chestionar != null)
            {
               var raspAnimal = await db.RaspAnimals.Where(c => c.idRasp == client.idRaspChestionar).ToListAsync();
                if(raspAnimal.Any())
                {
                    db.RaspAnimals.RemoveRange(raspAnimal);
                    await db.SaveChangesAsync();
                }
                 
                db.RaspChestionars.Remove(chestionar);
                await db.SaveChangesAsync();
               
            }

            client.idRaspChestionar = 0;

            await db.SaveChangesAsync();

            return RedirectToAction("Chestionar");
        }
    }
}
