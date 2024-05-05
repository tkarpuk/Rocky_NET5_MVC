using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;
using Rocky.Utility;
using System.Collections.Generic;
using System.Linq;

namespace Rocky.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;

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
