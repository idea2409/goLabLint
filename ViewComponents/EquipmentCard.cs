using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using golablint.Models;
using golablint.Data;

namespace goLabLint.wwwroot.ViewComponents
{
    public class EquipmentCard : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public EquipmentCard(ApplicationDbContext db) {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? limit) {
            if(limit != null)
                ViewBag.equipmentList = _db.Equipment.FromSqlRaw($"SELECT * FROM \"Equipment\" LIMIT {limit}");
            else
                ViewBag.equipmentList = _db.Equipment.FromSqlRaw("SELECT * FROM \"Equipment\"");
            return await Task.FromResult((IViewComponentResult)View("Card"));
        }
    }
}