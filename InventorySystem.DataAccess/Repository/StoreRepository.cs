using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models;

namespace InventorySystem.DataAccess.Repository
{
    public class StoreRepository : Repository<Store>, IStoreRepository
    {
        private readonly ApplicationDbContext _db;

        public StoreRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Store store)
        {
            var storeBD = _db.Stores.FirstOrDefault(s => s.Id == store.Id);

            if(storeBD != null)
            {
                storeBD.Name = store.Name;
                storeBD.Description = store.Description;
                storeBD.State = store.State;
                _db.SaveChanges();
            }
        }
    }
}