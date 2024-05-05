using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;
using Rocky.Models.ViewModels;
using Rocky.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Rocky.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            List<ShoppingCart> cart = HttpContext.Session.Get<List<ShoppingCart>>(GeneralConstant.SessionCart) 
                ?? new List<ShoppingCart>();

            List<int> ids = cart.Select(i => i.ProductId).ToList();

            List<Product> prodList = _db.Product.Where(p => ids.Contains(p.Id)).ToList();

            return View(prodList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            return RedirectToAction(nameof(Summary));
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.Claims.FirstOrDefault();

            List<ShoppingCart> cart = HttpContext.Session.Get<List<ShoppingCart>>(GeneralConstant.SessionCart)
                ?? new List<ShoppingCart>();
            List<int> ids = cart.Select(i => i.ProductId).ToList();
            List<Product> prodList = _db.Product.Where(p => ids.Contains(p.Id)).ToList();

            ProductUserVM productUserVM = new ProductUserVM()
            {
                ApplicationUser = _db.ApplicationUser.FirstOrDefault(u => u.Id == claim.Value),
                ProductList = prodList
            };

            return View(productUserVM);
        }

        public IActionResult Remove(int id)
        {
            List<ShoppingCart> cart = HttpContext.Session.Get<List<ShoppingCart>>(GeneralConstant.SessionCart)
                ?? new List<ShoppingCart>();

            cart.Remove(cart.FirstOrDefault(x => x.ProductId == id));
            HttpContext.Session.Set<List<ShoppingCart>>(GeneralConstant.SessionCart, cart);

            return RedirectToAction(nameof(Index));
        }
    }
}
