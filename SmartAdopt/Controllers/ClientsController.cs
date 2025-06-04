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

        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Index", "Home");
            var client = await db.Clients.FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);
            if (client.CompletedProfile == false)
            {
                TempData["ErrorMessage"] = "Va rugam sa va completati tot profilul!";
                ViewBag.Message = TempData["ErrorMessage"];
                return View(user);
            }

            if (TempData.ContainsKey("SuccessMessage"))
            {
                ViewBag.Message = TempData["SuccessMessage"];
                return View(user);
            }
            return View(user);
        }

        [Authorize(Roles = "Client")]
        public async Task<IActionResult> EditNr()
        {
            var userId = _userManager.GetUserId(User);
            var client = await db.Clients.FirstOrDefaultAsync(s => s.ApplicationUserId == userId);
            if (string.IsNullOrEmpty(userId))
            {
                return new ChallengeResult(); 
            }
            if (client == null) return RedirectToAction("Index", "Home");

            if (TempData.ContainsKey("ErrorMessage"))
            {
                ViewBag.Message = TempData["ErrorMessage"];
                return View(client);
            }
            return View(client);
        }

        [Authorize(Roles = "Client")]
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
                if (newclient == null) return RedirectToAction("Index", "Home");
                newclient.nr_telefon = client.nr_telefon;
                if (!string.IsNullOrEmpty(client.nr_telefon) && !string.IsNullOrEmpty(client.adresa))
                {
                    newclient.CompletedProfile = true;
                    db.SaveChanges();
                }

                if (!ECorectNr(client.nr_telefon))
                {
                    TempData["ErrorMessage"] = "Ati introdus un nr de telefon gresit";
                    return RedirectToAction("EditNr", "Clients");
                }
                bool ECorectNr(string nr)
                {
                    if (string.IsNullOrWhiteSpace(nr))
                        return false;

                    return System.Text.RegularExpressions.Regex.IsMatch(nr, @"^07\d{8}$");
                }

                db.SaveChanges();
                TempData["SuccessMessage"] = "Numar de telefon updatat";
                return RedirectToAction("Index", "Clients");
            }
        }

        [Authorize(Roles = "Client")]
        public async Task<IActionResult> EditAdresa()
        {
            var userId = _userManager.GetUserId(User);
            var client = await db.Clients.FirstOrDefaultAsync(s => s.ApplicationUserId == userId);
            if (string.IsNullOrEmpty(userId))
            {
                return new ChallengeResult();
            }
            if (client == null) return RedirectToAction("Index", "Home");

            if (TempData.ContainsKey("ErrorMessage"))
            {
                ViewBag.Message = TempData["ErrorMessage"];
                return View(client);
            }
            return View(client);
        }

        [Authorize(Roles = "Client")]
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
                if (newclient == null) return RedirectToAction("Index", "Home");
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
                TempData["SuccessMessage"] = "Adresa personala updatata";
                return RedirectToAction("Index", "Clients");
            }
        }

        [Authorize(Roles = "Client")]
        public async Task<IActionResult> CreareComanda(int id)
        {
            var animal = await db.Animals.FindAsync(id);
            if (animal == null)
            {
                return RedirectToAction("Index", "Animals");
            }

            var userId = _userManager.GetUserId(User);
            var client = await db.Clients.FirstOrDefaultAsync(s => s.ApplicationUserId == userId);
            if (client == null) return RedirectToAction("Index", "Home");

            var comanda = new Comanda
            {
                idAnimal = id,
                idClient = client.idClient, 
                data_comenzii = DateTime.Now,
                stare = "In asteptare", 
                total_plata = animal.pret, 
                metoda_platii = "", 
                motivatie = "" 
            };
            ViewBag.nume = animal.nume;
            return View(comanda);
        }

        [Authorize(Roles = "Client")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreareComanda(Comanda comanda)
        {

            var userId = _userManager.GetUserId(User);
            var client = await db.Clients.FirstOrDefaultAsync(s => s.ApplicationUserId == userId);
            if (client == null) return RedirectToAction("Index", "Home");

            comanda.idClient = client.idClient; 
            comanda.data_comenzii = DateTime.Now;
            comanda.stare = "In asteptare";
            db.Comandas.Add(comanda);
            await db.SaveChangesAsync();

            TempData["message"] = "Comanda a fost plasata";
            return RedirectToAction("Index", "Animals");
        }

        [Authorize(Roles = "Client")]
        public async Task<IActionResult> ShowComenzi()
        {
            var userId = _userManager.GetUserId(User);
            var client = await db.Clients.FirstOrDefaultAsync(s => s.ApplicationUserId == userId);
            if (client == null) return RedirectToAction("Index", "Home");

            int clientId = client.idClient; 
            var comenzi = await db.Comandas.Where(c => c.idClient == clientId).Include(c => c.Animal).ToListAsync();

            return View(comenzi);
        }

        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Chestionar()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Index", "Home");
            var client = await db.Clients.FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);
            if (client == null) return RedirectToAction("Index", "Home");

            if (client.idRaspChestionar == 0)
            {
                ViewBag.Chestionar = 0;
                return View(new ChestionarViewModel());
            }
            else
            {
                client = await db.Clients.Include(c => c.RaspChestionar).ThenInclude(rc => rc.RaspAnimals).ThenInclude(ra => ra.Animal).FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

                var animaleRec = await db.RaspChestionars.Where(rc => rc.idRasp == client.idRaspChestionar).SelectMany(rc => rc.RaspAnimals)
                    .Select(ra => ra.Animal)
                    .Take(5)
                    .ToListAsync();

                ViewBag.Chestionar = 1;
                ViewBag.AnimaleRec = animaleRec;
                return View();
            }
            
        }

        [Authorize(Roles = "Client")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Chestionar(ChestionarViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var client = await db.Clients.FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);
            if (client == null) return RedirectToAction("Index", "Home");

            var animaleRec = await GetAnimaleRecomandate(model);

            var raspChestionar = new RaspChestionar { idClient = client.idClient };
            db.RaspChestionars.Add(raspChestionar);
            await db.SaveChangesAsync(); 

            client.idRaspChestionar = raspChestionar.idRasp;
            db.Clients.Update(client); 
            await db.SaveChangesAsync(); 

            foreach (var animal in animaleRec)
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

        [Authorize(Roles = "Client")]
        private async Task<List<Animal>> GetAnimaleRecomandate(ChestionarViewModel model)
        {
            var animale = await db.Animals.ToListAsync();

            //functie de calcul care ofera valori in functie de diferenta dintre ce ofera clientul si ce va cere animalul
            int CalculScor(int user, int animal)
            {
                int diff = Math.Abs(user - animal);
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

            var scoredAnimale = animale.Select(a => new
            {
                Animal = a,
                Score = 0 
                        // 1.Dupa casa si marimea animalului cu val 3 care delimiteaza
                        + (model.Locuinta == "Apartment" && a.marime <= 3 ? 10 : 0)
                        + (model.Locuinta == "House" && a.marime > 3 ? 10 : 0)

                        // 2.Dupa gradina si nivel_energie cu val 3 care delimiteaza
                        + (model.GradinaBool && a.nivel_energie >= 3 ? 10 : 0)
                        + (!model.GradinaBool && a.nivel_energie < 3 ? 10 : 0)

                        // 3.Cat timp de miscare se ofera animalului, deci atentie
                        + (model.TimpMiscare == "Less than 30 minutes" && a.nivel_atentie_necesara <= 2 ? 10 : 0)
                        + (model.TimpMiscare == "30 minutes to 1 hour" && a.nivel_atentie_necesara == 3 ? 10 : 0)
                        + (model.TimpMiscare == "More than 1 hour" && a.nivel_atentie_necesara >= 4 ? 10 : 0)

                        // 4.Daca are animale sau copii, se adapteaza mai greu animalul
                        + (model.AnimaleBool && model.CopiiBool && a.nivel_adaptabilitate >= 4 ? 10 : 0)
                        + ((model.AnimaleBool ^ model.CopiiBool) && (a.nivel_adaptabilitate == 2 || a.nivel_adaptabilitate == 3) ? 10 : 0)
                        + (!model.AnimaleBool && !model.CopiiBool && a.nivel_adaptabilitate == 1 ? 10 : 0)

                        // 5.Restul de valori se calculeaza strict dupa valoare
                        + CalculScor(model.Marime, a.marime)
                        + CalculScor(model.NivelAtentie, a.nivel_atentie_necesara)
                        + CalculScor(model.GrupVarsta, a.grupa_varsta)
                        + CalculScor(model.Adaptabilitate, a.nivel_adaptabilitate)
            })
            //primele 5 cu cel mai mare scor
            .OrderByDescending(x => x.Score)
            .Select(x => x.Animal)
            .Take(5)
            .ToList();

            return scoredAnimale;
        }

        [Authorize(Roles = "Client")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetChestionar()
        {
            var user = await _userManager.GetUserAsync(User);
            var client = await db.Clients.FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);
            if (client == null) return RedirectToAction("Index", "Home");
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
