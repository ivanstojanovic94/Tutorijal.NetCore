using Bravissimo.Web.Data;
using Bravissimo.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;


namespace Bravissimo.Web.Controllers
{
    public class ApplicationTypeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ApplicationTypeController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<ApplicationType> objList = _db.ApplicationTypes;
            return View(objList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationType obj)
        {
            if (ModelState.IsValid)
            {
                _db.ApplicationTypes.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
            
        }
        public IActionResult Edit(int? Id)
        {
            if (Id == null || Id == 0)
                return NotFound();

            var obj = _db.ApplicationTypes.Find(Id);

            if (obj == null)
                return NotFound();

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationType obj)
        {
            if (ModelState.IsValid)
            {
                _db.ApplicationTypes.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0)
                return NotFound();

            var obj = _db.ApplicationTypes.Find(Id);

            if (obj == null)
                return NotFound();
            
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? Id)
        {
            var obj = _db.ApplicationTypes.Find(Id);
            
            if (obj == null)
                return NotFound();

            _db.ApplicationTypes.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        
    }
}
