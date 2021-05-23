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

        [Route("~/borrowing-list",Name="borrowing-list")]
        public IActionResult Index(string id) {
            Guid _id;
            if (!Guid.TryParse(id, out _id)) {
                return BadRequest();
            }
            var borrowingList = from borrowing in _db.Set<Borrowing>()
            join equipment in _db.Set<Equipment>()
            on borrowing.equipment.id equals equipment.id
            select new { borrowing, equipment };
            ViewBag.borrowingList = JObject.Parse(JsonConvert.SerializeObject(Json(borrowingList))).GetValue("Value");
            return View();
        }

    }
}