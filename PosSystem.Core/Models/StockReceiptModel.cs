using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PosSystem.Core.Models
{
    public class StockReceiptModel
    {
        [Key]
        public int StockReceiptID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public int QuantityReceived { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitCost { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCost { get; set; }

        [Required]
        public DateTime ReceiveDate { get; set; }

        [StringLength(100)]
        public string? Supplier { get; set; }

        [StringLength(50)]
        public string? BatchNumber { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation property
        [ForeignKey("ProductID")]
        public virtual ProductModel Product { get; set; }
    }
}