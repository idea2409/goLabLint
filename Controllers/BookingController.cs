using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using golablint.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace golablint.Controllers {
    public class BookingController : Controller {
        public IActionResult Index() {
            return View();
        }

        public IActionResult Equipment() {
            return View();
        }

        [Route("~/booking/equipment/{id}")]
        public IActionResult Equipment(Guid id,string name) {
            Console.WriteLine(name);
            return View();
        }
    }
}