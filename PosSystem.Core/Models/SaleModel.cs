using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosSystem.Core.Models
{
    public class SaleModel
    {
        [Key]
        public int SaleID { get; set; }

        [Required]
        public DateTime SaleDate { get; set; } = DateTime.Now;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = "Cash";

        [StringLength(100)]
        public string? CustomerName { get; set; }

        // Navigation property for sale items
        public virtual ICollection<SaleItemModel> SaleItems { get; set; } = new List<SaleItemModel>();

        // Helper property for display
        [NotMapped]
        public int TotalItems => SaleItems?.Sum(item => item.Quantity) ?? 0;
    }
}
