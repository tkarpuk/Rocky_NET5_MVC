using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;
using System.Collections.Generic;

namespace Rocky.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext applicationDbContext)
        {
            _db = applicationDbContext;   
        }

        public IActionResult Index()
        {
            IEnumerable<Category> objList = _db.Category;
            return View(objList);
        }

        // GET Create
        public IActionResult Create()
        {
            return View();
        }

        // POST Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                _db.Category.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        // GET Edit
        public IActionResult Edit(int? id)
        {
            if (id is null || id < 1)
            {
                return NotFound();
            }

            var obj = _db.Category.Find(id);
            if (obj is null)
            {
                return NotFound();
            }

            return View(obj);
        }

        // POST Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _db.Category.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        // GET Edit
        public IActionResult Delete(int? id)
        {
            if (id is null || id < 1)
            {
                return NotFound();
            }

            var obj = _db.Category.Find(id);
            if (obj is null)
            {
                return NotFound();
            }

            return View(obj);
        }

        // POST Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.Category.Find(id);
            if (obj is null)
            {
                return NotFound();
            }

            _db.Category.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
