﻿using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models
{
    public class Brand
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(60, ErrorMessage = "The name must be a maximum of 60 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(100, ErrorMessage = "The description must be a maximum of 100 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "State is required")]
        public bool State { get; set; }
    }
}