using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystem.Models
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(80)]
        public string Names { get; set; }

        [Required(ErrorMessage = "Lastname is required")]
        [MaxLength(80)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [MaxLength(200)]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required")]
        [MaxLength(60)]
        public string City { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [MaxLength(60)]
        public string Country { get; set; }

        [NotMapped]
        public string Role { get; set; }
    }
}