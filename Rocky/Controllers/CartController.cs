using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Rocky.Data;
using Rocky.Models;
using Rocky.Models.ViewModels;
using Rocky.Utility;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

        public CartController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(ProductUserVM productUserVM) 
        { 
            //string pathToTemplate = _hostEnvironment.ContentRootPath + Path.DirectorySeparatorChar.ToString()
            //    + "templates" + Path.DirectorySeparatorChar.ToString()
            //    + "Inquiry.html";

            var pathTemplate = $"{_webHostEnvironment.WebRootPath}\\templates\\Inquiry.html";
            string subject = "New inquery";
            string htmlBody = string.Empty;
            using (StreamReader sr = new StreamReader(pathTemplate))
            {
                htmlBody = sr.ReadToEnd();
            }

            StringBuilder productListSB = new StringBuilder();
            foreach (var product in productUserVM.ProductList)
            {
                productListSB.Append(
                    $" - Name: {product.Name} <span style='font-size:14px;'>(ID: {product.Id})</span><br />");
            }

            string messageBody = string.Format(htmlBody,
                productUserVM.ApplicationUser.FullName,
                productUserVM.ApplicationUser.Email,
                productUserVM.ApplicationUser.PhoneNumber,
                productListSB.ToString());

            await _emailSender.SendEmailAsync(GeneralConstant.EmailAdmin, subject, messageBody);

            return RedirectToAction(nameof(InquireConfirmation)); 
        }

        public IActionResult InquireConfirmation()
        {
            HttpContext.Session.Clear();
            return View();
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
