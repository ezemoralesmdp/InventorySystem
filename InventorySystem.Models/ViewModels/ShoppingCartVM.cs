namespace InventorySystem.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public Company Company { get; set; }
        public Product Product { get; set; }
        public int Stock { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
    }
}