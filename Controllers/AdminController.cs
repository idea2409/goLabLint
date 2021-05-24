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
        public IActionResult Add(string name, string amount, List<IFormFile> files, string description) {

            EquipmentController equipmentController = new EquipmentController(_db);
            int _amount;
            bool isError = false;
            if (string.IsNullOrEmpty(description) || description == "none") {
                isError = true;
                ModelState.AddModelError("description", "กรุณาระบุห้องปฏิบัตการ");
            }
            if (!Int32.TryParse(amount, out _amount) || string.IsNullOrEmpty(amount)) {
                Console.WriteLine("error");
                Console.WriteLine(amount);
                isError = true;
                ModelState.AddModelError("amount", "กรุณาระบุจำนวนที่ถูกต้อง");
            }
            if (string.IsNullOrEmpty(name)) {
                isError = true;
                ModelState.AddModelError("name", "กรุณาระบุชื่ออุปกรณ์");
            }
            string image = equipmentController.ConvertImageToString(files);
            if (!image.StartsWith("data:image/jpeg")) {
                isError = true;
                ModelState.AddModelError("image", image);
            }
            Equipment equipment = new Equipment();
            equipment.id = Guid.NewGuid();
            equipment.amount = _amount;
            equipment.description = description;
            equipment.image = image;
            equipment.name = name;
            ViewBag.equipment = equipment;
            if (isError) {
                Console.WriteLine("error");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                var errorJSON = JsonConvert.SerializeObject(errorList);
                ViewBag.errors = JsonConvert.DeserializeObject(errorJSON);
                Console.WriteLine(errorJSON);
                return View();
            }
            History history = new History();
            history.id = Guid.NewGuid();
            history.issueDate = DateTime.Now;
            history.status = "Create";
            history.amount = _amount;
            history.equipmentid = equipment.id;
            if (!TryValidateModel(history, nameof(history))) {
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                var errorJSON = JsonConvert.SerializeObject(errorList);
                ViewBag.errors = JsonConvert.DeserializeObject(errorJSON);
                return View();
            }
            return RedirectToRoute(new {
                controller = "admin",
                    action = "equipment",
            });
        }

        [Route("~/admin/borrowing-list", Name = "admin-borrowing")]
        public IActionResult BorrowingList() {
            return View();
        }

        [Route("~/api/get-borrowing-list")]
        public JsonResult getBorrowingList() {
            var borrowingList = (from borrowing in _db.Set<Borrowing>() join equipment in _db.Set<Equipment>() on borrowing.equipment.id equals equipment.id join user in _db.Set<User>() on borrowing.user.id equals user.id select new { borrowing, equipment, user });
            return Json(borrowingList);
        }
    }

}