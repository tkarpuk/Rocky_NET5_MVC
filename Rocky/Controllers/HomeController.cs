﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rocky.Data;
using Rocky.Models;
using Rocky.Models.ViewModels;
using Rocky.Utility;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Rocky.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                Products = _db.Product.Include(c => c.Category).Include(a => a.ApplicationType),
                Categories = _db.Category
            };

            return View(homeVM);
        }

        public IActionResult Details(int id)
        {
            DetailsVM detailsVM = new DetailsVM()
            {
                Product = _db.Product
                .Include(c => c.Category)
                .Include(a => a.ApplicationType)
                .FirstOrDefault(p => p.Id == id),
            };

            List<ShoppingCart> shoppingCarts = HttpContext.Session.Get<List<ShoppingCart>>(GeneralConstant.SessionCart)
                ?? new List<ShoppingCart>();

            detailsVM.ExistsInCart = shoppingCarts.Any(s => s.ProductId == id);

            return View(detailsVM);
        }

        [HttpPost, ActionName("Details")]        
        public IActionResult DetailsPost(int id)
        {
            List<ShoppingCart> shoppingCarts = HttpContext.Session.Get<List<ShoppingCart>>(GeneralConstant.SessionCart) 
                ?? new List<ShoppingCart>();

            shoppingCarts.Add(new ShoppingCart() { ProductId = id });
            HttpContext.Session.Set<List<ShoppingCart>>(GeneralConstant.SessionCart, shoppingCarts);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveFromCart(int id)
        {
            List<ShoppingCart> shoppingCarts = HttpContext.Session.Get<List<ShoppingCart>>(GeneralConstant.SessionCart)
                ?? new List<ShoppingCart>();

            ShoppingCart removeProduct = shoppingCarts.SingleOrDefault(x => x.ProductId == id);
            if (removeProduct != null)
            {
                shoppingCarts.Remove(removeProduct);
                HttpContext.Session.Set<List<ShoppingCart>>(GeneralConstant.SessionCart, shoppingCarts);
            }            

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
