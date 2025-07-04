﻿using Microsoft.AspNetCore.Authorization;
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
        public IActionResult Index(int? page)
        {
            int pageSize = 6; 
            int pageNumber = page ?? 1;
            if (pageNumber < 1) pageNumber = 1; 

            var search = "";
            var animals = db.Animals.OrderBy(a => a.pret).ThenBy(a => a.nume);
            //searchBar
            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                //scot spatii libere
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
                //cautare in animal
                List<int> animalIds = db.Animals.Where(p => p.nume.Contains(search) || p.rasa.Contains(search) || p.specie.Contains(search) || p.descriere.Contains(search) || Convert.ToString(p.varsta).Contains(search) || p.vaccinuri.Contains(search)).Select(a => a.idAnimal).ToList();

                animals = db.Animals.Where(animal => animalIds.Contains(animal.idAnimal)).OrderBy(a => a.pret).ThenBy(a => a.nume);
            }

            int totalItems = animals.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            if (pageNumber > totalPages && totalPages > 0) pageNumber = totalPages; 

            var animals2 = animals
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.SearchString = search;
            ViewBag.Animals = animals2;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }
    }
}
