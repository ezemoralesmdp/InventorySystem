using InventorySystem.Models;

namespace InventorySystem.DataAccess.Repository.IRepository
{
    public interface IInventoryDetailRepository : IRepository<InventoryDetail>
    {
        void Update(InventoryDetail inventoryDetail);
    }
}