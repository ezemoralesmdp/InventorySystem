using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Models
{
    public class StoreProduct
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StoreId { get; set; }

        [ForeignKey(nameof(StoreId))]
        public Store Store { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        public int Amount { get; set; }
    }
}