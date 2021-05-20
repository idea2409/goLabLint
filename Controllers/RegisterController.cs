using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using golablint.Data;
using golablint.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace golablint.Controllers {
    public class RegisterController : Controller {
        private readonly ApplicationDbContext _db;
        public RegisterController(ApplicationDbContext db) {
            _db = db;
        }
        public IActionResult Index() {
            if (HttpContext.User.Identity.IsAuthenticated)
                return RedirectToRoute(new {
                    controller = "Home",
                        action = "Index",
                });
            return View();
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public IActionResult Index(string name, string surname, string email, string password, string confirmPassword) {
            User user = new User();
            user.id = Guid.NewGuid();
            user.name = name;
            user.surname = surname;
            user.email = email;
            user.password = password;
            user.confirmPassword = confirmPassword;
            var re = new Regex("^\\d+$");
            user.role = user.email != null && user.email.IndexOf('@') != -1 && re.Matches(user.email.Remove(user.email.IndexOf('@'))).Count > 0 ? "นักศึกษา" : "อาจารย์";
            ViewBag.user = user;
            if (!TryValidateModel(user, nameof(user))) {
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                var errorJSON = JsonConvert.SerializeObject(errorList);
                ViewBag.errors = JsonConvert.DeserializeObject(errorJSON);
                return View();
            }
            if (_db.User.FromSqlRaw($"SELECT * FROM \"User\" WHERE email = \'{user.email}\' LIMIT 1").Count() > 0) {
                ModelState.AddModelError("email", "อีเมลนี้ถูกใช้งานไปแล้ว");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                var errorJSON = JsonConvert.SerializeObject(errorList);
                ViewBag.errors = JsonConvert.DeserializeObject(errorJSON);
                return View();
            }
            user.password = BCrypt.Net.BCrypt.HashPassword(user.password);
            user.confirmPassword = BCrypt.Net.BCrypt.HashPassword(user.confirmPassword);
            _db.User.Add(user);
            _db.SaveChanges();
            return RedirectToRoute(new {
                controller = "Login",
                    action = "Index",
            });
        }
    }
}