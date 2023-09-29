using InventorySystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventorySystem.DataAccess.Repository.IRepository
{
    public interface IStoreProductRepository : IRepository<StoreProduct>
    {
        void Update(StoreProduct storeProduct);
    }
}