using InventorySystem.DataAccess.Repository.IRepository;

namespace InventorySystem.DataAccess.Repository
{
    public class UnitWork : IUnitWork
    {
        private readonly ApplicationDbContext _db;
        public IStoreRepository Store { get; private set; }

        public UnitWork(ApplicationDbContext db)
        {
            _db = db;
            Store = new StoreRepository(_db);
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}