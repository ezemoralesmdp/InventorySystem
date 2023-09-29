using InventorySystem.Models;

namespace InventorySystem.DataAccess.Repository.IRepository
{
    public interface IKardexInventoryRepository : IRepository<KardexInventory> 
    {
        Task RegisterKardex(int storeProductId, string type, string detail, int stockPrevious, int amount, string userId);
    }
}