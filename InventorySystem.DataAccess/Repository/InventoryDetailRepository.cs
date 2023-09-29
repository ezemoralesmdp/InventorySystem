using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models;

namespace InventorySystem.DataAccess.Repository
{
    public class InventoryDetailRepository : Repository<InventoryDetail>, IInventoryDetailRepository
    {
        private readonly ApplicationDbContext _db;

        public InventoryDetailRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(InventoryDetail inventoryDetail)
        {
            var inventoryDetailDB = _db.InventoryDetails.FirstOrDefault(s => s.Id == inventoryDetail.Id);

            if(inventoryDetailDB != null)
            {
                inventoryDetailDB.PreviousStock = inventoryDetail.PreviousStock;
                inventoryDetailDB.Amount = inventoryDetail.Amount;
                _db.SaveChanges();
            }
        }
    }
}