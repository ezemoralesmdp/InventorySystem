using InventorySystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventorySystem.DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product product);
        IEnumerable<SelectListItem> GetAllDropDownList(string obj);
    }
}