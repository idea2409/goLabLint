using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using golablint.Data;
using golablint.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace golablint.Controllers {
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller {
        private readonly ApplicationDbContext _db;
        public AdminController(ApplicationDbContext db) {
            _db = db;
        }

        public IActionResult Index() {
            return View();
        }

        [Route("~/admin/equipment")]
        public IActionResult Equipment(string search = "") {
            ViewBag.search = search;
            return View();
        }

        [Route("~/admin/equipment/{id}", Name = "admin-equipment")]
        public IActionResult Describe(string id) {
            Guid _id;
            if (!Guid.TryParse(id, out _id)) {
                return BadRequest();
            }
            var equipment = _db.Equipment.FromSqlRaw($"SELECT * FROM \"Equipment\" WHERE id = \'{id}\' LIMIT 1").OrderBy(item => item.id).FirstOrDefault();
            if (equipment == null) return NotFound();
            ViewBag.equipment = equipment;
            return View();
        }

        [Route("~/admin/equipment/add")]
        public IActionResult Add() {
            return View();
        }

        [HttpPost]
        [Route("~/admin/equipment/add", Name = "create-equipment")]
        public async Task<IActionResult> Add([FromForm] IFormFile file, string name, string amount, string description) {
            EquipmentController equipmentController = new EquipmentController(_db);
            int _amount;
            bool isError = false;
            if (string.IsNullOrEmpty(description) || description == "none") {
                isError = true;
                ModelState.AddModelError("description", "กรุณาระบุห้องปฏิบัตการ");
            }
            if (!Int32.TryParse(amount, out _amount) || string.IsNullOrEmpty(amount)) {
                isError = true;
                ModelState.AddModelError("amount", "กรุณาระบุจำนวนที่ถูกต้อง");
            }
            if (string.IsNullOrEmpty(name)) {
                isError = true;
                ModelState.AddModelError("name", "กรุณาระบุชื่ออุปกรณ์");
            }
            string base64 = equipmentController.ConvertImageToString(file);
            if (!base64.StartsWith("data:image/jpeg")) {
                isError = true;
                ModelState.AddModelError("image", base64);
            }
            Equipment equipment = new Equipment();
            equipment.id = Guid.NewGuid();
            equipment.amount = _amount;
            equipment.description = description;
            equipment.image = base64;
            equipment.name = name;
            ViewBag.equipment = equipment;
            if (isError) {
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                var errorJSON = JsonConvert.SerializeObject(errorList);
                ViewBag.errors = JsonConvert.DeserializeObject(errorJSON);
                return View();
            }
            History history = new History();
            history.id = Guid.NewGuid();
            history.issueDate = DateTime.Now;
            history.status = "Create";
            history.amount = _amount;
            history.equipmentid = equipment.id;
            history.equipment = equipment;
            if (!TryValidateModel(history, nameof(history))) {
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                var errorJSON = JsonConvert.SerializeObject(errorList);
                ViewBag.errors = JsonConvert.DeserializeObject(errorJSON);
                return View();
            }
            _db.Equipment.Add(equipment);
            _db.History.Add(history);
            await _db.SaveChangesAsync();
            return RedirectToRoute(new {
                controller = "admin",
                    action = "equipment",
            });
        }

        [HttpPost]
        [Route("~/api/adjust-equipment", Name = "adjust-equipment")]
        public async Task<IActionResult> adjustEquipment(string id, string ac, [FromForm] string amount) {
            Guid _id;
            int _amount;
            bool isError = false;
            if (!Guid.TryParse(id, out _id)) {
                isError = true;
                ModelState.AddModelError("equipmentId", "หมายเลขอุปกรณ์ไม่ถูกต้อง");
            }
            if (!Int32.TryParse(amount, out _amount) || string.IsNullOrEmpty(amount)) {
                isError = true;
                ModelState.AddModelError("amount", "จำนวนไม่ถูกต้อง");
            }
            if (isError) {
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                var errorJSON = JsonConvert.SerializeObject(errorList);
                ViewBag.errors = JsonConvert.DeserializeObject(errorJSON);
                return RedirectToAction("Describe", new { id = _id });
            }
            var equipment = _db.Equipment.FromSqlRaw($"SELECT * FROM \"Equipment\" WHERE id = \'{id}\' LIMIT 1").OrderBy(item => item.id).FirstOrDefault();
            if (equipment == null) {
                ModelState.AddModelError("equipmentId", "ไม่พบอุปกรณ์ดังกล่าวในระบบ");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                var errorJSON = JsonConvert.SerializeObject(errorList);
                ViewBag.errors = JsonConvert.DeserializeObject(errorJSON);
                return RedirectToAction("Describe", new { id = _id });
            };
            if (ac == "delete") {
                equipment.amount = (equipment.amount - _amount < 0) ? 0 : (equipment.amount - _amount);
            } else if (ac == "add") {
                equipment.amount += _amount;
            }
            History history = new History();
            history.id = new Guid();
            history.issueDate = DateTime.Now;
            history.amount = _amount;
            history.equipment = equipment;
            history.equipmentid = _id;
            history.status = ac == "add" ? "Add" : "Subtract";
            if (!TryValidateModel(history, nameof(history))) {
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                var errorJSON = JsonConvert.SerializeObject(errorList);
                ViewBag.errors = JsonConvert.DeserializeObject(errorJSON);
                return RedirectToAction("Describe", new { id = _id });
            }
            _db.Equipment.Update(equipment);
            _db.History.Add(history);
            await _db.SaveChangesAsync();
            return RedirectToAction("Describe", new { id = _id });
        }

        [Route("~/admin/borrowing-list", Name = "admin-borrowing")]
        public IActionResult BorrowingList() {
            return View();
        }
        [Route("~/api/get-user")]
        public JsonResult getUser() {
            return Json((from user in _db.User where user.role == "นักศึกษา" select user).ToList());
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
        public JsonResult getBorrowingList(string name = "") {
            if (string.IsNullOrEmpty(name)) {
                var borrowingList = (from borrowing in _db.Set<Borrowing>() join equipment in _db.Set<Equipment>() on borrowing.equipment.id equals equipment.id join user in _db.Set<User>() on borrowing.user.id equals user.id select new { borrowing, equipment, user });
                return Json(borrowingList);
            } else {
                var borrowingList = (from borrowing in _db.Set<Borrowing>() join equipment in _db.Set<Equipment>() on borrowing.equipment.id equals equipment.id join user in _db.Set<User>() on borrowing.user.id equals user.id where user.name == name select new { borrowing, equipment, user });
                return Json(borrowingList);
            }
        }
        [HttpPost]
        [Route("~/api/delete-equipment")]
        public async Task<IActionResult> delete(string id) {
            Guid _id;
            if (!Guid.TryParse(id, out _id)) {
                ModelState.AddModelError("equipmentId", "หมายเลขอุปกรณ์ไม่ถูกต้อง");
            }
            var equipment = _db.Equipment.FromSqlRaw($"SELECT * FROM \"Equipment\" WHERE id = \'{id}\' LIMIT 1").OrderBy(item => item.id).FirstOrDefault();
            if (equipment == null) {
                ModelState.AddModelError("equipmentId", "ไม่พบอุปกรณ์ดังกล่าวในระบบ");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                var errorJSON = JsonConvert.SerializeObject(errorList);
                ViewBag.errors = JsonConvert.DeserializeObject(errorJSON);
                return RedirectToAction("equipment");
            };
            _db.Equipment.Remove(equipment);
            await _db.SaveChangesAsync();
            return RedirectToAction("equipment");
        }
    }

}