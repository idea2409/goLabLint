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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace golablint.Controllers {
    public class BorrowingListController : Controller {
        private readonly ApplicationDbContext _db;
        public BorrowingListController(ApplicationDbContext db) {
            _db = db;
        }

        [Route("~/borrowing-list", Name = "borrowing-list")]
        public IActionResult Index() {
            return View();
        }

        [Route("~/api/get-borrowing-list/{id}")]
        public JsonResult getBorrowingList(string id) {
            Guid _id;
            if (!Guid.TryParse(id, out _id)) {
                ModelState.AddModelError("userId", "หมายเลขผู้ใช้งานไม่ถูกต้อง");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                return Json(errorList);
            }
            var borrowingList = from borrowing in _db.Set<Borrowing>()
            join equipment in _db.Set<Equipment>()
            on borrowing.equipment.id equals equipment.id
            select new { borrowing, equipment };
            return Json(borrowingList);
        }

        [HttpPost]
        [Route("~/api/set-complete",Name="set-complete")]
        public JsonResult setCompleted(string id) {
            Guid _id;
            if (!Guid.TryParse(id, out _id)) {
                ModelState.AddModelError("borrowingId", "หมายเลขการจองไม่ถูกต้อง");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                return Json(errorList);
            }
            var borrowingList = _db.Borrowing.FromSqlRaw($"UPDATE \"Borrowing\" SET status = 'Completed' WHERE id = \'{_id}\'");
            if(borrowingList.Count() == 0) {
                ModelState.AddModelError("borrowingId", "ไม่พบการจองดังกล่าวในระบบ");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                return Json(errorList);
            }
            // _db.SaveChanges();
            return Json(borrowingList);
        }
    }
}