using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebAppCmvc.Data;
using WebAppCmvc.Models;

namespace WebAppCmvc.Controllers
{
    public class CategoryController : Controller
    {
        readonly AppDbContext _db;

        public CategoryController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("username"))) return RedirectToAction("Index", "Login");

            List<Category> categoryList = _db.Categories.ToList();
            return View(categoryList);
        }
        
        //GET
        public IActionResult Create()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("username"))) return RedirectToAction("Index", "Login");
            else if (HttpContext.Session.GetString("username") != "manager") return RedirectToAction("Index", "Category");

            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category ob)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("username"))) return RedirectToAction("Index", "Login");
            else if (HttpContext.Session.GetString("username") != "manager") return RedirectToAction("Index", "Category");

            if (ModelState.IsValid)
            {
                List<Category> catList = _db.Categories.Where(c => c.Name == ob.Name).ToList();
                if (catList.Count == 0)
                {
                    _db.Categories.Add(ob);
                    _db.SaveChanges();
                    TempData["success"] = $"{ob.Name} inserted successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("SameCategoryError", "Same category is already in database.");
                }
            }
            return View(ob);
        }



        private static string TypeName="MEH";

        //GET
        public IActionResult Edit(int? id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("username"))) return RedirectToAction("Index", "Login");
            else if (HttpContext.Session.GetString("username") != "manager") return RedirectToAction("Index", "Category");

            if (id == null || id == 0) return NotFound();
            Category category = _db.Categories.Find(id);
            if (category == null) return NotFound();
            TypeName = category.Name;
            return View(category);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category ob)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("username"))) return RedirectToAction("Index", "Login");
            else if (HttpContext.Session.GetString("username") != "manager") return RedirectToAction("Index", "Category");

            if (ModelState.IsValid)
            {
                List<Category> catList = _db.Categories.Where(c => c.Name == ob.Name).ToList();
                if (catList.Count == 0)
                {
                    List<Product> prods = _db.Products.Where(p => p.Type == TypeName).ToList();
                    foreach(Product p in prods) p.Type = ob.Name;
                    _db.Categories.Update(ob);
                    _db.SaveChanges();
                    TypeName = "MEH";
                    TempData["success"] = $"Updated to {ob.Name} successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("SameCategoryError", "Same category is already in database.");
                }
            }
            return View(ob);
        }

        //GET
        public IActionResult Delete(int? id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("username"))) return RedirectToAction("Index", "Login");
            else if (HttpContext.Session.GetString("username") != "manager") return RedirectToAction("Index", "Category");

            if (id == null || id == 0) return NotFound();
            Category category = _db.Categories.Find(id);
            if (category == null) return NotFound();
            TypeName = category.Name;
            return View(category);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(Category ob)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("username"))) return RedirectToAction("Index", "Login");
            else if (HttpContext.Session.GetString("username") != "manager") return RedirectToAction("Index", "Category");

            ob = _db.Categories.Find(ob.Id);
            if (ob == null) return NotFound();
            List<Product> prods = _db.Products.Where(p => p.Type == ob.Name).ToList();
            if(prods.Count == 0)
            {
                _db.Categories.Remove(ob);
                _db.SaveChanges();
                TempData["success"] = $"{ob.Name} deleted successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("ProductOfCategoryExist",
                    $"Cannot delete category {TypeName} while one or more products of this category exists. " +
                    $"Deleting category {TypeName} will make {prods.Count} product(s) invalid. " +
                    $"Try deleting all products of this category first and then try again.");
                return View("Delete", ob);
            }
        }
    }
}
