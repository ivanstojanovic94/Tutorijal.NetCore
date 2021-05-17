using Bravissimo.Web.Data;
using Bravissimo.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bravissimo.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> objList = _db.Products;

            foreach(var obj in objList)
            {
                obj.Category = _db.Categories.FirstOrDefault(s => s.Id == obj.CategoryId);
            }
            return View(objList);
        }

        public IActionResult Upsert(int? Id)
        {
            IEnumerable<SelectListItem> CategoryDropDown = _db.Categories.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });

            ViewBag.CategoryDropDown = CategoryDropDown;

            Product product = new Product();
            if(Id == null)
            {
                return View(product);
            }
            else
            {
                product = _db.Products.Find(Id);
                if(product == null)
                {
                    return NotFound();
                }

                return View(product);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Product obj)
        {
            if (ModelState.IsValid)
            {
                _db.Products.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }
    }
}
