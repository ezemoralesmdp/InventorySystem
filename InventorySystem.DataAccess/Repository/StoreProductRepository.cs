using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models;

namespace InventorySystem.DataAccess.Repository
{
    public class StoreProductRepository : Repository<StoreProduct>, IStoreProductRepository
    {
        private readonly ApplicationDbContext _db;

        public StoreProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(StoreProduct storeProduct)
        {
            var storeProductDB = _db.StoresProducts.FirstOrDefault(s => s.Id == storeProduct.Id);

            if(storeProductDB != null)
            {
                storeProductDB.Amount = storeProduct.Amount;
                _db.SaveChanges();
            }
        }
    }
}