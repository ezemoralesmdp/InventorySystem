using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystem.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Serial number is required")]
        [MaxLength(60)]
        public string SerialNumber { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(60)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Cost is required")]
        public double Cost { get; set; }

        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "State is required")]
        public bool State { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }

        [Required(ErrorMessage = "Brand is required")]
        public int BrandId { get; set; }

        [ForeignKey(nameof(BrandId))]
        public Brand Brand { get; set; }

        public int? FatherId { get; set; }

        public virtual Product Father { get; set; }
    }
}