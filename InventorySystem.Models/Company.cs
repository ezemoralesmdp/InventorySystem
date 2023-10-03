using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystem.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(80)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(200)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [MaxLength(60)]
        public string Country { get; set; }

        [Required(ErrorMessage = "City is required")]
        [MaxLength(60)]
        public string City { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [MaxLength(100)]
        public string Address { get; set; }

        [Required(ErrorMessage = "Telephone is required")]
        [MaxLength(40)]
        public string Telephone { get; set; }

        [Required(ErrorMessage = "Store Sell is required")]
        public int StoreSellId { get; set; }

        [ForeignKey(nameof(StoreSellId))]
        public Store Store { get; set; }

        public string CreatedById { get; set; }

        [ForeignKey(nameof(CreatedById))]
        public User CreatedBy { get; set; }

        public string UpdatedById { get; set; }

        [ForeignKey(nameof(UpdatedById))]
        public User UpdatedBy { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}