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
    public class EquipmentController : Controller {

        ApplicationDbContext _db;
        public EquipmentController(ApplicationDbContext db) {
            _db = db;
        }
        public IActionResult Index() {
            return View();
        }
        [Route("~/api/equipment")]
        public JsonResult getEquipment(int? limit) {

            if (limit.HasValue) {
                return (Json(_db.Equipment.FromSqlRaw($"SELECT * FROM \"Equipment\" LIMIT {limit}")));

            } else {
                return (Json(_db.Equipment.FromSqlRaw("SELECT * FROM \"Equipment\"")));
            }
        }

        [Route("~/api/equipment/{id}")]
        public JsonResult getEquipment(string id) {
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
            return Json(equipmentData);
        }
        [Route("~/api/image-to-string")]
        public string ConvertImageToString(List<IFormFile> files) {
            if (files.Count != 1) return "กรุณาส่งมาแค่ไฟล์เดียว";
            string extension = System.IO.Path.GetExtension(files[0].FileName);
            if (extension != ".jpg" && extension != ".png") return "กรุณาส่งไฟล์ประเภท .jpg หรือ .png เท่านั้น";
            MemoryStream ms = new MemoryStream();
            files[0].CopyTo(ms);
            return string.Format("data:image/jpeg;base64,{0}", Convert.ToBase64String(ms.ToArray()));
        }
    }
}