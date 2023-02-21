using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
                User credential = _db.Users.Where(x => x.Username == u.Username && x.Password == Sidekick.Crypt(u.Password)).FirstOrDefault();
                //User credential = _db.Users.Where(x => x.Username == u.Username && x.Password == u.Password).FirstOrDefault();
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

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(User u)
        {
            if (ModelState.IsValid)
            {
                User credential = _db.Users.Where(x => x.Username == u.Username).FirstOrDefault();
                if (credential == null)
                {
                    u.Password = Sidekick.Crypt(u.Password);
                    _db.Users.Add(u);
                    _db.SaveChanges();
                    ViewBag.SignupSuccessMessage = $"Registered user {u.Username} successfully.";
                }
                else
                {
                    ViewBag.SignupErrorMessage = $"Username {u.Username} is already taken. Try a different name.";
                }
            }
            return View(u);
        }


        public IActionResult Profile(string? id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("username"))) return RedirectToAction("Index", "Login");

            if (string.IsNullOrEmpty(id)) return NotFound();
            User user = _db.Users.Where(x => x.Username == id).FirstOrDefault();
            if (user == null) return NotFound();
            //ViewData["FUserState"] = $"{user.Id} : {user.Username} : {user.Password}";
            return View(user);
        }

        [HttpPost]
        public IActionResult Profile(User u)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("username"))) return RedirectToAction("Index", "Login");

            //ViewData["UserState"] = $"{u.Id} : {u.Username} : {u.Password}";

            if (ModelState.IsValid)
            {
                u.Password = Sidekick.Crypt(u.Password);
                _db.Users.Update(u);
                _db.SaveChanges();
                ViewData["ProfileSaved"] = "Profile settings saved.";
            }
            else ViewData["ProfileError"] = "Profile settings error!";

            return View(u);
        }
    }
}

