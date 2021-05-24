using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using golablint.Data;
using golablint.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace golablint.Controllers {
    public class BookingController : Controller {

        private readonly ApplicationDbContext _db;
        public BookingController(ApplicationDbContext db) {
            _db = db;
        }

        [Route("~/booking/{id}")]
        public IActionResult Equipment(string id) {
            Guid _id;
            if (!Guid.TryParse(id, out _id)) {
                return BadRequest();
            }
            var equipment = _db.Equipment.FromSqlRaw($"SELECT * FROM \"Equipment\" WHERE id = \'{id}\' LIMIT 1");
            if (equipment.Count() == 0) return NoContent();
            var equipmentData = equipment.OrderBy(item => item.id).FirstOrDefault();
            ViewBag.availableTime = JObject.Parse(JsonConvert.SerializeObject(getAvailable(id))).GetValue("Value");
            ViewBag.equipment = equipmentData;
            return View();
        }

        [Route("~/api/get-available")]
        public JsonResult getAvailable(string id, string date = "") {
            Guid _id;
            if (!Guid.TryParse(id, out _id)) {
                ModelState.AddModelError("equipmentId", "หมายเลขอุปกรณ์ไม่ถูกต้อง");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                return Json(errorList);
            }
            var equipment = _db.Equipment.FromSqlRaw($"SELECT * FROM \"Equipment\" WHERE id = \'{id}\' LIMIT 1");
            if (equipment.Count() == 0) {
                ModelState.AddModelError("equipmentId", "อุปกรณ์ดังกล่าวไม่มีข้อมูลอยู่ในระบบ");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                return Json(errorList);
            };
            var equipmentData = equipment.OrderBy(item => item.id).FirstOrDefault();
            DateTime start;
            if (date == "") {
                start = DateTime.Today;
            } else {
                if (!DateTime.TryParseExact(date, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out start)) {
                    ModelState.AddModelError("date", "วันที่ไม่ตรงตามรูปแบบ yyyy-MM-dd");
                    var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                    return Json(errorList);
                }
            }
            List<DateTime> availableTime = new List<DateTime>();
            var clockQuery = from offset in Enumerable.Range(9, 8)
            select TimeSpan.FromHours(offset);
            foreach (var time in clockQuery) {
                availableTime.Add(start.Add(time));
            }
            IDictionary<string, List<int>> available = new Dictionary<string, List<int>>();
            List<int> available_amount = new List<int>();
            foreach (var _startDate in availableTime) {
                var total = equipmentData.amount;
                var totalBooking = _db.Borrowing.FromSqlRaw($"SELECT * FROM \"Borrowing\" WHERE equipmentId = \'{_id}\' AND \"startDate\" = \'{_startDate.ToString("yyyy-MM-dd HH:mm:ss")}\'").Count();
                available_amount.Add(total - totalBooking);
            }
            available.Add("time", available_amount);
            return Json(available);
        }

        [HttpPost]
        [Route("~/api/book")]
        public Object Book(string userId, string equipmentId, string startDate, string endDate, int amount = 1) {
            Guid _userId, _equipmentId;
            if (!Guid.TryParse(userId, out _userId)) {
                ModelState.AddModelError("userId", "หมายเลขผู้ใช้งานไม่ถูกต้อง");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                var errorJSON = JsonConvert.SerializeObject(errorList);
                return errorJSON;
            }
            if (!Guid.TryParse(equipmentId, out _equipmentId)) {
                ModelState.AddModelError("equipmentId", "หมายเลขอุปกรณ์ไม่ถูกต้อง");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                var errorJSON = JsonConvert.SerializeObject(errorList);
                return errorJSON;
            }
            DateTime _startDate, _endDate;
            if (!DateTime.TryParseExact(startDate, "yyyy-MM-dd:HH", null, System.Globalization.DateTimeStyles.None, out _startDate)) {
                ModelState.AddModelError("startDate", "วันยืมไม่ตรงตามรูปแบบ yyyy-MM-dd:HH");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                var errorJSON = JsonConvert.SerializeObject(errorList);
                return errorJSON;
            }
            if (!DateTime.TryParseExact(endDate, "yyyy-MM-dd:HH", null, System.Globalization.DateTimeStyles.None, out _endDate)) {
                ModelState.AddModelError("endDate", "วันคืนไม่ตรงตามรูปแบบ yyyy-MM-dd:HH");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                var errorJSON = JsonConvert.SerializeObject(errorList);
                return errorJSON;
            }
            if (_endDate < _startDate) {
                ModelState.AddModelError("endDate", "กรุณาระบุวันคืนที่เหมาะสม");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                var errorJSON = JsonConvert.SerializeObject(errorList);
                return errorJSON;
            }
            if (DateTime.Now > _startDate) {
                ModelState.AddModelError("startDate", "กรุณาระบุวันยืมที่เหมาะสม");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                var errorJSON = JsonConvert.SerializeObject(errorList);
                return errorJSON;
            }
            if ((_endDate - _startDate).TotalHours != 1) {
                ModelState.AddModelError("endDate", "กรุณาระบุช่วงเวลายืมไม่เกิน 1 ชั่วโมง");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                var errorJSON = JsonConvert.SerializeObject(errorList);
                return errorJSON;
            }
            var equipment = _db.Equipment.FromSqlRaw($"SELECT * FROM \"Equipment\" WHERE id = \'{equipmentId}\' LIMIT 1");
            if (equipment.Count() == 0) {
                ModelState.AddModelError("equipmentId", "อุปกรณ์ดังกล่าวไม่มีข้อมูลอยู่ในระบบ");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                var errorJSON = JsonConvert.SerializeObject(errorList);
                return errorJSON;
            }
            var user = _db.User.FromSqlRaw($"SELECT * FROM \"User\" WHERE id = \'{userId}\' LIMIT 1");
            if (user.Count() == 0) {
                ModelState.AddModelError("userId", "ผู้ใช้ดังกล่าวไม่มีข้อมูลอยู่ในระบบ");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                var errorJSON = JsonConvert.SerializeObject(errorList);
                return errorJSON;
            }
            var userData = user.OrderBy(item => item.id).FirstOrDefault();
            if (userData.role == "อาจารย์") {
                ModelState.AddModelError("userId", "ผู้ใช้ที่เป็นอาจารย์ไม่สามารถทำรายการจองอุปกรณ์ได้");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                var errorJSON = JsonConvert.SerializeObject(errorList);
                return errorJSON;
            }
            if (userData.status == "Blacklist") {
                ModelState.AddModelError("userId", "ผู้ใช้ที่ถูกอายัติไม่สามารถทำรายการจองอุปกรณ์ได้");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                var errorJSON = JsonConvert.SerializeObject(errorList);
                return errorJSON;
            }
            var equipmentData = equipment.OrderBy(item => item.id).FirstOrDefault();
            var total = equipmentData.amount;
            var totalBooking = _db.Borrowing.FromSqlRaw($"SELECT * FROM \"Borrowing\" WHERE equipmentId = \'{_equipmentId}\' AND \"startDate\" = \'{_startDate.ToString("yyyy-MM-dd HH:mm:ss",CultureInfo.CreateSpecificCulture("en-US"))}\'").Count();
            if (total - totalBooking - amount <= 0) {
                ModelState.AddModelError("equipmentId", "ไม่สามารถทำการจองได้");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                var errorJSON = JsonConvert.SerializeObject(errorList);
                return errorJSON;
            }
            Borrowing borrowing = new Borrowing();
            borrowing.id = Guid.NewGuid();
            borrowing.user = userData;
            borrowing.equipment = equipmentData;
            borrowing.startDate = _startDate;
            borrowing.endDate = _endDate;
            borrowing.status = "Ongoing";
            if (!ModelState.IsValid) {
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                var errorJSON = JsonConvert.SerializeObject(errorList);
                return errorJSON;
            }
            _db.Borrowing.Add(borrowing);
            _db.SaveChanges();
            return borrowing;
        }

    }
}