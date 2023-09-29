using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystem.Models
{
    public class Inventory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [Required]
        public DateTime InitialDate { get; set; }

        [Required]
        public DateTime FinalDate { get; set; }

        [Required(ErrorMessage = "Selection of a store is required.")]
        public int StoreId { get; set; }

        [ForeignKey(nameof(StoreId))]
        public Store Store { get; set; }

        [Required]
        public bool State { get; set; }
    }
}