using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystem.Models
{
    public class KardexInventory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StoreProductId { get; set; }

        [ForeignKey(nameof(StoreProductId))]
        public StoreProduct StoreProduct { get; set; }

        [Required]
        [MaxLength(100)]
        public string Type { get; set; } // Entrada - Salida

        [Required]
        public string Detail { get; set; }

        public int PreviousStock { get; set; }

        [Required]
        public int Amount { get; set; }

        [Required]
        public double Cost { get; set; }

        [Required]
        public int Stock { get; set; }

        public double Total { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public DateTime RegistrationDate { get; set; }
    }
}