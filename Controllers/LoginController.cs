using Microsoft.AspNetCore.Mvc;
using WebAppCmvc.Data;
using WebAppCmvc.Models;

namespace WebAppCmvc.Controllers
{
    public class LoginController : Controller
    {
        readonly AppDbContext _db;

        public LoginController(AppDbContext db)
        {
            _db = db;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(User u)
        {
            if(ModelState.IsValid)
            {
                var credential = _db.Users.Where(x => x.Username == u.Username && x.Password == u.Password).FirstOrDefault();
                if(credential == null)
                {
                    ViewBag.LoginErrorMessage = "*Login failed";
                    return View();
                }
                else
                {
                    //Session["username"] = u.Username;
                    HttpContext.Session.SetString("username", u.Username);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(u);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}
