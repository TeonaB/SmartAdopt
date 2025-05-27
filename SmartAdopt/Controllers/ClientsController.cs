using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartAdopt.Data;
using SmartAdopt.Models;

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
    }
}
