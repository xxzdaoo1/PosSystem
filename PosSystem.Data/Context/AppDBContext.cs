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
        public DbSet<PosSystem.Core.Models.ProductModel> Products { get; set; }
        public DbSet<PosSystem.Core.Models.SaleModel> Sales { get; set; }
        public DbSet<PosSystem.Core.Models.SaleItemModel> SaleItems { get; set; }
        public DbSet<PosSystem.Core.Models.StockAdjustmentModel> StockAdjustments { get; set; }
        public DbSet<PosSystem.Core.Models.StockReceiptModel> StockReceipts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {   // Database connection
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=PosSystemDB;User Id=admin;Password=admin123;TrustServerCertificate=true;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<ProductModel>(entity =>
            {
                entity.HasKey(e => e.ProductID);
                entity.Property(e => e.Barcode).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.StockQuantity).IsRequired();
            });

            modelBuilder.Entity<SaleModel>(entity =>
            {
                entity.HasKey(s => s.SaleID);
                entity.Property(s => s.SaleDate).IsRequired();
                entity.Property(s => s.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(s => s.PaymentMethod).HasMaxLength(50).IsRequired();
                entity.Property(s => s.CustomerName).HasMaxLength(100);

                // Relationship: Sale has many SaleItems
                entity.HasMany(s => s.SaleItems)
                      .WithOne(si => si.Sale)
                      .HasForeignKey(si => si.SaleID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SaleItemModel>(entity =>
            {
                entity.HasKey(si => si.SaleItemID);
                entity.Property(si => si.Quantity).IsRequired();
                entity.Property(si => si.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(si => si.TotalPrice).HasColumnType("decimal(18,2)").IsRequired();

                // Relationship: SaleItem belongs to a Sale
                entity.HasOne(si => si.Sale)
                      .WithMany(s => s.SaleItems)
                      .HasForeignKey(si => si.SaleID)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relationship: SaleItem references a Product
                entity.HasOne(si => si.Product)
                      .WithMany()
                      .HasForeignKey(si => si.ProductID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<StockReceiptModel>(entity =>
            {
                entity.HasKey(sr => sr.StockReceiptID);
                entity.Property(sr => sr.QuantityReceived).IsRequired();
                entity.Property(sr => sr.UnitCost).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(sr => sr.TotalCost).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(sr => sr.ReceiveDate).IsRequired();
                entity.Property(sr => sr.Supplier).HasMaxLength(100);
                entity.Property(sr => sr.BatchNumber).HasMaxLength(50);
                entity.Property(sr => sr.Notes).HasMaxLength(500);

                // Relationship: StockReceipt references a Product
                entity.HasOne(sr => sr.Product)
                      .WithMany()
                      .HasForeignKey(sr => sr.ProductID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<StockAdjustmentModel>(entity =>
            {
                entity.HasKey(sa => sa.StockAdjustmentID);
                entity.Property(sa => sa.QuantityChange).IsRequired();
                entity.Property(sa => sa.AdjustmentType).HasMaxLength(50).IsRequired();
                entity.Property(sa => sa.AdjustmentDate).IsRequired();
                entity.Property(sa => sa.Supplier).HasMaxLength(100);
                entity.Property(sa => sa.Reason).HasMaxLength(200);
                entity.Property(sa => sa.Reference).HasMaxLength(50);
                entity.Property(sa => sa.Notes).HasMaxLength(500);

                // Relationship: StockAdjustment references a Product
                entity.HasOne(sa => sa.Product)
                      .WithMany()
                      .HasForeignKey(sa => sa.ProductID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

        }
    }
}
