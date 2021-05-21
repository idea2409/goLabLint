using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using golablint.Data;
using golablint.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace golablint.Controllers {
    public class AdminController : Controller {
        private readonly ApplicationDbContext _db;
        public AdminController(ApplicationDbContext db) {
            _db = db;
        }
        public IActionResult Index() {
            return View();
        }

        public IActionResult Equipment() {
            return View();
        }

        [Route("~/admin/equipment/{id}")]
        public IActionResult Describe(Guid id) {
            var equipment = _db.Equipment.FromSqlRaw($"SELECT * FROM \"Equipment\" WHERE id = \'{id}\' LIMIT 1").OrderBy(item => item.id).FirstOrDefault();
            if(equipment == null) return BadRequest();
            ViewBag.equipment = equipment;
            return View();
        }

        [Route("~/admin/equipment/add")]
        public IActionResult Add() {
            return View();
        }

        // [Route("~/admin/equipment/add")]
        // public IActionResult Add(Guid id)
        // {
        //     return View();
        // }
    }
}