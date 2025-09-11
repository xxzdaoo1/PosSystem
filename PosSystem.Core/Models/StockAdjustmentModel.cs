using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PosSystem.Core.Models
{
    public class StockAdjustmentModel
    {
        [Key]
        public int StockAdjustmentID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public int QuantityChange { get; set; }

        [Required]
        [StringLength(50)]
        public string AdjustmentType { get; set; } // "Receive", "Adjustment", "Sale", "Damage"

        [Required]
        public DateTime AdjustmentDate { get; set; }

        [StringLength(100)]
        public string? Supplier { get; set; }

        [StringLength(200)]
        public string? Reason { get; set; }

        [StringLength(50)]
        public string? Reference { get; set; } // Receipt ID, Sale ID, etc.

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation property
        [ForeignKey("ProductID")]
        public virtual ProductModel Product { get; set; }
    }
}