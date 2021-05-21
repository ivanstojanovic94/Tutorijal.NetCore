﻿using Bravissimo.Web.Data;
using Bravissimo.Web.Models;
using Bravissimo.Web.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bravissimo.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> objList = _db.Products;

            foreach(var obj in objList)
            {
                obj.Category = _db.Categories.FirstOrDefault(s => s.Id == obj.CategoryId);
                obj.ApplicationType = _db.ApplicationTypes.FirstOrDefault(i => i.Id == obj.ApplicationTypeId);
            }
            return View(objList);
        }

        public IActionResult Upsert(int? Id)
        {
            /*IEnumerable<SelectListItem> CategoryDropDown = _db.Categories.Select(i => new SelectListItem
             {
             Text = i.Name,
             Value = i.Id.ToString()
             });

             //ViewBag.CategoryDropDown = CategoryDropDown;

             ViewData["CategoryDropDown"] = CategoryDropDown;

             Product product = new Product();*/


            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectLIst = _db.Categories.Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString()
                }),
                ApplicationTypeSelectList = _db.ApplicationTypes.Select(i => new SelectListItem {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            if(Id == null)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _db.Products.Find(Id);
                if(productVM.Product == null)
                {
                    return NotFound();
                }

                return View(productVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if(productVM.Product.Id == 0)
                {
                    string upload = webRootPath + WC.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName+extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    productVM.Product.Image = fileName + extension;

                    _db.Products.Add(productVM.Product);
                }
                else
                {

                    var objFromDb = _db.Products.AsNoTracking().FirstOrDefault(i => i.Id == productVM.Product.Id);

                    if(files.Count() > 0)
                    {
                        string upload = webRootPath + WC.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var oldFile = Path.Combine(upload, objFromDb.Image);

                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using(var fileStram = new FileStream(Path.Combine(upload, fileName+extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStram);
                        }
                        
                        productVM.Product.Image = fileName + extension;

                    }
                    else
                    {
                        productVM.Product.Image = objFromDb.Image;
                    }
                    _db.Products.Update(productVM.Product);
                    
                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            productVM.CategorySelectLIst = _db.Categories.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            productVM.ApplicationTypeSelectList = _db.ApplicationTypes.Select(s => new SelectListItem 
            {
                Text = s.Name,
                Value = s.Id.ToString()
            });

            return View(productVM);
        }
        public IActionResult Delete(int? Id)
        {
            if (Id == null && Id == 0)
                return NotFound();

            Product product = _db.Products.Include(s => s.Category).Include(i => i.ApplicationType).FirstOrDefault(e => e.Id == Id);
           //  product.Category = _db.Categories.Find(product.CategoryId);
            if (product == null)
                return NotFound();

            return View(product);

        }
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? Id)
        {
            var obj = _db.Products.Find(Id);
            if (obj == null)
                return NotFound();

            string upload = _webHostEnvironment.WebRootPath + WC.ImagePath;
            var oldFile = Path.Combine(upload, obj.Image);

            if (System.IO.File.Exists(oldFile))
                System.IO.File.Delete(oldFile);

            _db.Products.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
