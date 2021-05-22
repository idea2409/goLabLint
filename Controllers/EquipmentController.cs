using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using golablint.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using golablint.Data;

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
        public Object getEquipment() {
            var equipment = _db.Equipment.FromSqlRaw($"SELECT * FROM \"Equipment\"").OrderBy(item => item.id).FirstOrDefault();
            return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(equipment));
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