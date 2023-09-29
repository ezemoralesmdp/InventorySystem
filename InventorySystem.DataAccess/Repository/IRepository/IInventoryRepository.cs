using InventorySystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventorySystem.DataAccess.Repository.IRepository
{
    public interface IInventoryRepository : IRepository<Inventory>
    {
        void Update(Inventory inventory);
        IEnumerable<SelectListItem> GetAllDropDownList(string obj);
    }
}