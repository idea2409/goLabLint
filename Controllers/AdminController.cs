using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using golablint.Data;
using golablint.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace golablint.Controllers {
    [Authorize(Roles="Admin")]
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

        [Route("~/admin/borrowing-list")]
        public IActionResult BorrowingList() {
            ViewBag.borrowingList = JObject.Parse(JsonConvert.SerializeObject(getBorrowingList())).GetValue("Value");
            return View();
        }
        [Route("~/api/get-user")]
        public JsonResult getUser(string status = "Normal") {
            return Json((from user in _db.User where user.status == status && user.role == "นักศึกษา" select user).ToList());
        }
        
        [Route("~/admin/blacklist")]
        public IActionResult Blacklist() {
            return View(); 
        }
        [HttpPost]
        [Route("~/api/blacklist")]
        public async Task<IActionResult> setBlacklist([FromQuery]string id,[FromQuery]string ac) {
            Console.WriteLine(id);
            Console.WriteLine(ac);
            Guid _id;
            if(!Guid.TryParse(id, out _id)) {
                return BadRequest();
            }
            var user = (from u in _db.User where u.id == _id select u).OrderBy(a => a.id).Take(1).FirstOrDefault();
            if(user == null) {
                return NotFound();
            }
            user.status = ac == "to" ? "Blacklist" : "Normal";
            Console.WriteLine(Json(user));
            _db.User.Update(user);
            await _db.SaveChangesAsync();
            return RedirectToAction("Blacklist");
        }

        [Route("~/api/get-borrowing-list")]
        public JsonResult getBorrowingList(string orderBy = "equipmentName") {
            var borrowingList = from borrowing in _db.Set<Borrowing>()
                                join equipment in _db.Set<Equipment>()
                                on borrowing.equipment.id equals equipment.id
                                join user in _db.Set<User>()
                                on borrowing.user.id equals user.id
                                orderby (orderBy=="equipmentName" ? "equipment.name" : orderBy=="userName" ? "user.name" : orderBy)
                                select new { borrowing, equipment, user };
            return Json(borrowingList);
        }
    }
}