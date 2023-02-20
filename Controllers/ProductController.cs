using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebAppCmvc.Data;
using WebAppCmvc.Models;

namespace WebAppCmvc.Controllers
{
    public class ProductController : Controller
    {
        readonly AppDbContext _db;

        public ProductController(AppDbContext db)
        {
            _db = db;
        }


        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("username"))) return RedirectToAction("Index", "Login");

            List<Product> tmplist = Sidekick.ShuffleList(_db.Products.ToList());
            List<Product> prodList = new();
            int i = 0;
            foreach(Product x in tmplist)
            {
                prodList.Add(x);
                i++;
                if (i == 15) break;
            }
            return View(prodList);
        }

        //GET
        public IActionResult Create()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("username"))) return RedirectToAction("Index", "Login");
            else if (HttpContext.Session.GetString("username") != "manager") return RedirectToAction("Index", "Product");

            ProdCatg pc = new ProdCatg();
            var ctl = _db.Categories.ToList();
            foreach(var c in ctl)
            {
                pc.catgs.Add(new SelectListItem { Text = c.Name, Value = c.Name});
            }
            return View(pc);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProdCatg ob)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("username"))) return RedirectToAction("Index", "Login");
            else if (HttpContext.Session.GetString("username") != "manager") return RedirectToAction("Index", "Product");


            if (ModelState.IsValid)
            {
                List<Product> prodList = _db.Products.Where(p => p.Name == ob.prod.Name && p.Type == ob.prod.Type).ToList();
                if (prodList.Count == 0)
                {
                    _db.Products.Add(ob.prod);
                    _db.SaveChanges();
                    TempData["success"] = $"{ob.prod.Name} inserted successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("SameProductError", "Same product is already in database.");
                }
            }

            ob = new ProdCatg();
            var ctl = _db.Categories.ToList();
            foreach (var c in ctl)
            {
                ob.catgs.Add(new SelectListItem { Text = c.Name, Value = c.Name });
            }
            return View(ob);
        }



        private static int TmpProdId = 0;

        //GET
        public IActionResult Edit(int? id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("username"))) return RedirectToAction("Index", "Login");
            else if (HttpContext.Session.GetString("username") != "manager") return RedirectToAction("Index", "Product");

            if (id == null || id == 0) return NotFound();
            TmpProdId = (int)id;
            Product prod = _db.Products.Find(TmpProdId);
            if (prod == null) return NotFound();

            ProdCatg pc = new ProdCatg();
            pc.prod = prod;
            var ctl = _db.Categories.ToList();
            foreach (var c in ctl)
            {
                pc.catgs.Add(new SelectListItem { Text = c.Name, Value = c.Name });
            }
            return View(pc);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProdCatg ob)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("username"))) return RedirectToAction("Index", "Login");
            else if (HttpContext.Session.GetString("username") != "manager") return RedirectToAction("Index", "Product");

            if (ModelState.IsValid)
            {
                ob.prod.Id= TmpProdId;
                _db.Products.Update(ob.prod);
                _db.SaveChanges();
                TmpProdId = 0;
                TempData["success"] = $"{ob.prod.Name} updated successfully.";
                return RedirectToAction("Index");
            }

            var ctl = _db.Categories.ToList();
            foreach (var c in ctl)
            {
                ob.catgs.Add(new SelectListItem { Text = c.Name, Value = c.Name });
            }
            return View(ob);
        }



        //GET
        public IActionResult Delete(int? id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("username"))) return RedirectToAction("Index", "Login");
            else if (HttpContext.Session.GetString("username") != "manager") return RedirectToAction("Index", "Product");

            if (id == null || id == 0) return NotFound();
            Product product = _db.Products.Find(id);
            if (product == null) return NotFound();
            return View(product);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("username"))) return RedirectToAction("Index", "Login");
            else if (HttpContext.Session.GetString("username") != "manager") return RedirectToAction("Index", "Product");

            Product product = _db.Products.Find(id);
            if (product == null) return NotFound();
            _db.Products.Remove(product);
            _db.SaveChanges();
            TempData["success"] = $"{product.Name} deleted successfully.";
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("username"))) return RedirectToAction("Index", "Login");


            Product p = _db.Products.Find(id);
            if(p == null) return NotFound();
            return View(p);
        }


        public IActionResult Search(string searchTerm = "*", string searchType = "*", int minPrice = 0, int maxPrice = int.MaxValue, bool sortDesc = false, string sortBys= "Pid")
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("username"))) return RedirectToAction("Index", "Login");


            ProdListCatg plc = new ProdListCatg();
            //plc.prods = _db.Products.Where(x => x.Name.Contains(searchTerm) && x.Type == searchType).ToList();
            if (string.IsNullOrEmpty(searchTerm))
            {
                if (string.IsNullOrEmpty(searchType))
                {
                    if (sortBys == "Name") plc.prods = _db.Products.Where(x => x.Price >= minPrice && x.Price <= maxPrice).OrderBy(x => x.Name).ToList();
                    else if (sortBys == "Price") plc.prods = _db.Products.Where(x => x.Price >= minPrice && x.Price <= maxPrice).OrderBy(x => x.Price).ToList();
                    else plc.prods = _db.Products.Where(x => x.Price >= minPrice && x.Price <= maxPrice).ToList();
                }
                else
                {
                    if(sortBys == "Name") plc.prods = _db.Products.Where(x => x.Type == searchType && x.Price >= minPrice && x.Price <= maxPrice).OrderBy(x => x.Name).ToList();
                    else if (sortBys == "Price") plc.prods = _db.Products.Where(x => x.Type == searchType && x.Price >= minPrice && x.Price <= maxPrice).OrderBy(x => x.Price).ToList();
                    else plc.prods = _db.Products.Where(x => x.Type == searchType && x.Price >= minPrice && x.Price <= maxPrice).ToList();
                }
            }
            else
            {
                if (string.IsNullOrEmpty(searchType))
                {
                    if (sortBys == "Name") plc.prods = _db.Products.Where(x => x.Name.Contains(searchTerm) || x.Type.ToLower() == searchTerm.ToLower() && x.Price >= minPrice && x.Price <= maxPrice).OrderBy(x => x.Name).ToList();
                    else if (sortBys == "Price") plc.prods = _db.Products.Where(x => x.Name.Contains(searchTerm) || x.Type.ToLower() == searchTerm.ToLower() && x.Price >= minPrice && x.Price <= maxPrice).OrderBy(x => x.Price).ToList();
                    else plc.prods = _db.Products.Where(x => x.Name.Contains(searchTerm) || x.Type.ToLower() == searchTerm.ToLower() && x.Price >= minPrice && x.Price <= maxPrice).ToList();
                }
                else
                {
                    if (sortBys == "Name") plc.prods = _db.Products.Where(x => x.Name.Contains(searchTerm) && x.Type == searchType && x.Price >= minPrice && x.Price <= maxPrice).OrderBy(x => x.Name).ToList();
                    else if (sortBys == "Price") plc.prods = _db.Products.Where(x => x.Name.Contains(searchTerm) && x.Type == searchType && x.Price >= minPrice && x.Price <= maxPrice).OrderBy(x => x.Price).ToList();
                    else plc.prods = _db.Products.Where(x => x.Name.Contains(searchTerm) && x.Type == searchType && x.Price >= minPrice && x.Price <= maxPrice).ToList();
                }
            }

            if (sortDesc) plc.prods.Reverse();
            List<Category> ctl = _db.Categories.ToList();
            foreach (var c in ctl) plc.catgs.Add(new SelectListItem { Text = c.Name, Value = c.Name });
            return View(plc);
        }
    }
}
