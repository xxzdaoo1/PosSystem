using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PosSystem.Core.Models;

/*
Migrate the database using Entity Framework Core:
In Package Manager Console:
    Add-Migration InitialCreate
    Update-Database
*/

namespace PosSystem.Data.Context
{
    public class AppDBContext : DbContext
    {
        public DbSet<PosSystem.Core.Models.Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {   // Database connection
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=PosSystemDB;User Id=admin;Password=admin123;TrustServerCertificate=true;");
        }
    }
}
