using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rocky.Data;
using Rocky.Models;
using Rocky.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Rocky.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext applicationDbContext)
        {
            _db = applicationDbContext;   
        }

        public IActionResult Index()
        {
            IEnumerable<Product> objList = _db.Product;
            foreach(var obj in objList)
            {
                obj.Category = _db.Category.FirstOrDefault(c => c.Id == obj.CategoryId);
            }

            return View(objList);
        }

        // GET Upsert
        public IActionResult Upsert(int? id)
        {
            //IEnumerable<SelectListItem> CategoryDropDown = _db.Category
            //    .Select(c => new SelectListItem()
            //    {
            //        Text = c.Name,
            //        Value = c.Id.ToString()
            //    });
            //ViewBag.CategoryDropDown = CategoryDropDown;
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _db.Category.Select(c => 
                    new SelectListItem()
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    })
            };

            if (id == null)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _db.Product.Find(id);
                return View(productVM);
            }
        }
    }
}
