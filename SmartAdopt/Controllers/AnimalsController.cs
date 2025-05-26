using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartAdopt.Data;
using SmartAdopt.Models;

namespace SmartAdopt.Controllers
{
    public class AnimalsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext db;

        public AnimalsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var search = "";
            var animals = db.Animals.OrderBy(a => a.pret).ThenBy(a => a.nume);
            //MOTOR DE CAUTARE
            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                // eliminam spatiile libere
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
                // Cautare in produs (Title si Description)
                List<int> animalIds = db.Animals.Where(p => p.nume.Contains(search) || p.rasa.Contains(search) || p.specie.Contains(search) || p.descriere.Contains(search)).Select(a => a.idAnimal).ToList();

                // Lista produselor care contin cuvantul cautat
                animals = db.Animals.Where(animal => animalIds.Contains(animal.idAnimal)).OrderBy(a => a.pret).ThenBy(a => a.nume);
            }

            ViewBag.SearchString = search;
            ViewBag.Animals = animals;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }
    }
}
