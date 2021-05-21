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
    public class AdminController : Controller
    {
        public IActionResult Index()
        {            
            return View();
        }

        public IActionResult Equipment()
        {
            return View();
        }

        [Route("~/admin/equipment/{id}")]
        public IActionResult Describe(Guid id)
        {
            ViewBag.equipment = "eiei";
            return View();
        }

        [Route("~/admin/equipment/add")]
        public IActionResult Add()
        {
            return View();
        }

        // [Route("~/admin/equipment/add")]
        // public IActionResult Add(Guid id)
        // {
        //     return View();
        // }
    }
}