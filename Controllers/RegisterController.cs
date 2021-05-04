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
    public class RegisterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [Route("~/api/register")]
        [HttpPost]
        // [ValidateAntiForgeryToken]
        public IActionResult Index(string name, string surname, string email, string password) {
            // Console.WriteLine(id);
            if(!ModelState.IsValid) {
                return BadRequest();;
            }
            // User user = new User();
            // user.name = name;
            // user.surname = surname;
            // user.email = email;
            // user.password = password;
            return View();
        }
    }
}
