using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using golablint.Data;
using golablint.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace goLabLint.wwwroot.ViewComponents {
    public class EquipmentCard : ViewComponent {
        private readonly ApplicationDbContext _db;

        public EquipmentCard(ApplicationDbContext db) {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? limit, string role) {
            if (limit != null)
                ViewBag.equipmentList = _db.Equipment.FromSqlRaw($"SELECT * FROM \"Equipment\" LIMIT {limit}");
            else
                ViewBag.equipmentList = _db.Equipment.FromSqlRaw("SELECT * FROM \"Equipment\"");
            ViewBag.role = role;
            return await Task.FromResult((IViewComponentResult) View("Card"));
        }
    }
}