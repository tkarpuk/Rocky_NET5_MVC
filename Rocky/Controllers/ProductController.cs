using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rocky.Data;
using Rocky.Models;
using Rocky.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rocky.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext applicationDbContext, IWebHostEnvironment webHostEnvironment)
        {
            _db = applicationDbContext; 
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> objList = _db.Product;
            foreach(var obj in objList)
            {
                obj.Category = _db.Category.FirstOrDefault(c => c.Id == obj.CategoryId);
                obj.ApplicationType = _db.ApplicationType.FirstOrDefault(a => a.Id == obj.ApplicationTypeId);
            }

            return View(objList);
        }

        // GET Upsert
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _db.Category.Select(c => 
                    new SelectListItem()
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    }),
                ApplicationTypeSelectList = _db.ApplicationType.Select(a => 
                    new SelectListItem()
                    {
                        Text = a.Name,
                        Value = a.Id.ToString()
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

        // POST Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if (productVM.Product.Id < 1)
                {
                    // create
                    string uploadPath = webRootPath + GeneralConstant.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productVM.Product.Image = fileName + extension;
                    _db.Product.Add(productVM.Product);
                }
                else
                {
                    // update
                    var oldObj = _db.Product.AsNoTracking().FirstOrDefault(p => p.Id == productVM.Product.Id);

                    if (files.Count > 0)
                    {
                        string uploadPath = webRootPath + GeneralConstant.ImagePath;
                        string newFile = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        string oldFile = Path.Combine(uploadPath, oldObj.Image);
                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var fileStream = new FileStream(Path.Combine(uploadPath, newFile + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        productVM.Product.Image = newFile + extension;
                    }
                    else
                    {
                        productVM.Product.Image = oldObj.Image;
                    }

                    _db.Product.Update(productVM.Product);
                }

                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategorySelectList = _db.Category.Select(c =>
                    new SelectListItem()
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    });
                productVM.ApplicationTypeSelectList = _db.ApplicationType.Select(a =>
                    new SelectListItem()
                    {
                        Text = a.Name,
                        Value = a.Id.ToString()
                    });

                return View(productVM);
            }
        }

        // GET Delete
        public IActionResult Delete(int? id)
        {
            if (id is null || id < 1)
            {
                return NotFound();
            }

            Product product = _db.Product.Include(c => c.Category).Include(a => a.ApplicationType).FirstOrDefault(p => p.Id == id);
            if (product is null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.Product.Find(id);
            if (obj is null)
            {
                return NotFound();
            }

            string webRootPath = _webHostEnvironment.WebRootPath;
            string uploadPath = webRootPath + GeneralConstant.ImagePath;
            string fileName = Path.Combine(uploadPath, obj.Image);
            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }

            _db.Product.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
