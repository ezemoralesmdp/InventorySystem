namespace InventorySystem.Models.ViewModels
{
    public class KardexInventoryVM
    {
        public Product Product { get; set; }

        public IEnumerable<KardexInventory> KardexInventoryList { get; set; }

        public DateTime InitialDate { get; set; }

        public DateTime FinalDate { get; set; }
    }
}