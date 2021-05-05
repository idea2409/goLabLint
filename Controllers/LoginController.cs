using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using golablint.Models;

namespace golablint.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [Route("~/api/login")]
        [HttpPost]
        // [ValidateAntiForgeryToken]
        public JsonResult Index(string name, string surname, string email, string password) {
            User user = new User();
            user.id = Guid.NewGuid();
            user.name = name;
            user.surname = surname;
            user.email = email;
            user.password = password;
            // var re = new Regex("^\\d+$");
            // user.role = re.Matches(user.email.Remove(user.email.IndexOf('@'))).Count > 0 ? "นักศึกษา":"อาจารย์";
            if (!TryValidateModel(user, nameof(user))) {
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0,kvp.Key.IndexOf('.')+1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                return Json(errorList);
            }
            return Json(user);
        }
    }
}
