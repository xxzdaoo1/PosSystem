using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosSystem.Core.Models
{
    public class ProductModel
    {
        [Key]
        public int ProductID { get; set; }

        [Required]
        [StringLength(50)]
        public string Barcode { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        public DateTime? LastStockUpdate { get; set; }

        // Helper properties
        [NotMapped]
        public string StockStatus
        {
            get
            {
                if (StockQuantity <= 0) return "Out of Stock";
                if (StockQuantity <= 10) return "Low Stock"; // Fixed threshold of 10
                return "In Stock";
            }
        }

        [NotMapped]
        public string NameWithStock => $"{Name} (Current: {StockQuantity})";
    }
}
