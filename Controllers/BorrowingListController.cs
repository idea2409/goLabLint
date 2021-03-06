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

        [Route("~/borrowing-list/{id}", Name = "borrowing-list")]
        public IActionResult Index(string id) {
            Guid _id;
            if (!Guid.TryParse(id, out _id)) {
                return BadRequest();
            }
            return View();
        }

        [Route("~/api/get-borrowing-list/{id}")]
        public JsonResult getBorrowingList(string id) {
            Console.WriteLine(id);
            Guid _id;
            if (!Guid.TryParse(id, out _id)) {
                ModelState.AddModelError("userId", "หมายเลขผู้ใช้งานไม่ถูกต้อง");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                return Json(errorList);
            }
            var borrowingList = from borrowing in _db.Set<Borrowing>()
            join equipment in _db.Set<Equipment>()
            on borrowing.equipment.id equals equipment.id
            join user in _db.Set<User>()
            on borrowing.user.id equals user.id
            where user.id == _id
            select new { borrowing, equipment };
            return Json(borrowingList);
        }

        [HttpPost]
        [Route("~/api/set-complete", Name = "set-complete")]
        [Route("~/api/delete-borrowing", Name = "delete-borrowing")]
        public IActionResult delete([FromQuery] string id, [FromQuery] string action,[FromQuery] string r="") {
            Guid _id;
            if (!Guid.TryParse(id, out _id)) {
                ModelState.AddModelError("borrowingId", "หมายเลขการจองไม่ถูกต้อง");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                return Json(errorList);
            }
            var borrowingList = _db.Borrowing.FromSqlRaw($"SELECT * FROM \"Borrowing\"WHERE id = \'{_id}\' LIMIT 1");
            if (borrowingList.Count() == 0) {
                ModelState.AddModelError("borrowingId", "ไม่พบการจองดังกล่าวในระบบ");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                return Json(errorList);
            }
            var borrowingListData = borrowingList.OrderBy(item => item.id).FirstOrDefault();
            if (action == "confirm") {
                borrowingListData.status = DateTime.Now > borrowingListData.endDate ? "Late" : "Completed";
                _db.Borrowing.Update(borrowingListData);
            } else {
                _db.Borrowing.Remove(borrowingListData);
            }
            _db.SaveChanges();
            Guid userId;
            if(!string.IsNullOrEmpty(r))
            {
                if(!Guid.TryParse(r,out userId)) return BadRequest();
                return RedirectToRoute("borrowing-list",new {id = userId});
            }
            return RedirectToRoute("admin-borrowing");
        }
    }
}