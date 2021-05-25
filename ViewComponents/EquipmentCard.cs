using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using golablint.Controllers;
using golablint.Data;
using golablint.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace goLabLint.wwwroot.ViewComponents {
    public class EquipmentCard : ViewComponent {
        private readonly ApplicationDbContext _db;

        public EquipmentCard(ApplicationDbContext db) {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? limit, string role, string find = "") {
            BookingController bookingController = new BookingController(_db);
            var now = DateTime.Today;
            now = now.AddHours(DateTime.Now.Hour + 1);
            if (limit.HasValue) {
                var equipmentList = await _db.Equipment.OrderBy(e => e.description).Take(limit.Value).ToListAsync();
                if (equipmentList.Count() == 0) {
                    return null;
                };
                ViewBag.equipmentList = JObject.Parse(JsonConvert.SerializeObject(await bookingController.getAvailableWithDate(equipmentList, now))).GetValue("Value");
            } else if (find != "") {
                var equipmentList = await (from e in _db.Equipment where e.name.ToLower().StartsWith(find) orderby e.description select e).ToListAsync();
                ViewBag.equipmentList = JObject.Parse(JsonConvert.SerializeObject(await bookingController.getAvailableWithDate(equipmentList, now))).GetValue("Value");
                ViewBag.equipmentCount = equipmentList.Count();
            } else {
                var equipmentList = await _db.Equipment.OrderBy(equipment => equipment.description).ToListAsync();
                if (equipmentList.Count() == 0) {
                    return null;
                };
                ViewBag.equipmentList = JObject.Parse(JsonConvert.SerializeObject(await bookingController.getAvailableWithDate(equipmentList, now))).GetValue("Value");
            }
            ViewBag.role = role;
            return await Task.FromResult((IViewComponentResult) View("Card"));
        }

    }
}