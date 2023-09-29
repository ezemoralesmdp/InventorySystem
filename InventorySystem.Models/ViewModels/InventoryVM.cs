using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventorySystem.Models.ViewModels
{
    public class InventoryVM
    {
        public Inventory Inventory { get; set; }

        public InventoryDetail InventoryDetail { get; set; }

        public IEnumerable<InventoryDetail> InventoryDetails { get; set; }

        public IEnumerable<SelectListItem> StoreList { get; set; }
    }
}