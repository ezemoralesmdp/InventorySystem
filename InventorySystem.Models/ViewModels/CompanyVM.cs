using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventorySystem.Models.ViewModels
{
    public class CompanyVM
    {
        public Company Company { get; set; }
        public IEnumerable<SelectListItem> StoreList { get; set; }
    }
}