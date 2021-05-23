using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using golablint.Models;
using golablint.Data;
using Microsoft.EntityFrameworkCore;

namespace golablint.Controllers
{
    public class BorrowingListController : Controller
    {
        private readonly ApplicationDbContext _db;
        public BorrowingListController(ApplicationDbContext db) {
            _db = db;
        }

        [Route("~/borrowing-list/{id}")]
        public IActionResult Index(string id)
        {
            Guid _id;
            if(!Guid.TryParse(id,out _id)) {
                return BadRequest();
            }
            ViewBag.borrowingList = _db.Borrowing.FromSqlRaw($"SELECT * FROM \"Borrowing\" WHERE userId = \'{id}\'");     
            return View();
        }

    }
}
