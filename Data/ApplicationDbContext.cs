using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using golablint.Models;
using Microsoft.EntityFrameworkCore;

namespace golablint.Data {
    public class ApplicationDbContext : DbContext {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {

        }

        public DbSet<User> User { get; set; }
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<Borrowing> Borrowing { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<User>().HasData(new User { id = Guid.NewGuid(), name = "admin", surname = "golablint", email = "admin@kmitl.ac.th", password = BCrypt.Net.BCrypt.HashPassword("Go@min123"), role = "Admin" });
        }
    }
}