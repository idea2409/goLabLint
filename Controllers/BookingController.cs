using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var equipment = _db.Equipment.FromSqlRaw($"SELECT * FROM \"Equipment\" WHERE id = \'{id}\' LIMIT 1").OrderBy(item => item.id).FirstOrDefault();
            if (equipment == null) return BadRequest();
            ViewBag.equipment = equipment;
            return View();
        }

        [Route("~/api/book")]
        public Object Book([FromQuery] string userId, [FromQuery] string equipmentId, [FromQuery] string startDate, [FromQuery] string endDate, [FromQuery] int amount = 1) {
            Guid _userId, _equipmentId;
            if (!Guid.TryParse(userId, out _userId)) {
                ModelState.AddModelError("userId", "ผู้ใช้งานดังกล่าวไม่มีในระบบ");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                var errorJSON = JsonConvert.SerializeObject(errorList);
                return errorJSON;
            }
            if (!Guid.TryParse(equipmentId, out _equipmentId)) {
                ModelState.AddModelError("equipmentId", "อุปกรณ์ดังกล่าวไม่มีในระบบ");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                var errorJSON = JsonConvert.SerializeObject(errorList);
                return errorJSON;
            }
            DateTime _startDate, _endDate;
            if (!DateTime.TryParseExact(startDate, "yyyyMMddHH", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out _startDate)) {
                ModelState.AddModelError("startDate", "วันยืมไม่ตรงตามรูปแบบ yyyyMMddHH");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                var errorJSON = JsonConvert.SerializeObject(errorList);
                return errorJSON;
            }
            if (!DateTime.TryParseExact(endDate, "yyyyMMddHH", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out _endDate)) {
                ModelState.AddModelError("endDate", "วันคืนไม่ตรงตามรูปแบบ yyyyMMddHH");
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
            if (userData.role == "Admin") {
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
            Borrowing borrowing = new Borrowing();
            borrowing.id = Guid.NewGuid();
            borrowing.user = userData;
            borrowing.equipment = equipmentData;
            borrowing.startDate = _startDate;
            borrowing.endDate = _endDate;
            if (borrowing.endDate < borrowing.startDate) {
                ModelState.AddModelError("endDate", "กรุณาระบุวันคืนที่เหมาะสม");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                var errorJSON = JsonConvert.SerializeObject(errorList);
                return errorJSON;
            }
            if (!ModelState.IsValid) {
                Console.WriteLine("model error");
                var errorList = ModelState.Where(elem => elem.Value.Errors.Any()).ToDictionary(kvp => kvp.Key.Remove(0, kvp.Key.IndexOf('.') + 1), kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception.Message : e.ErrorMessage).ToArray());
                var errorJSON = JsonConvert.SerializeObject(errorList);
                return errorJSON;
            }
            // _db.Borrowing.Add(borrowing);
            // _db.SaveChanges();            
            return borrowing;
        }

    }
}