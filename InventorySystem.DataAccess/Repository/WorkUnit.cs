using InventorySystem.DataAccess.Repository.IRepository;

namespace InventorySystem.DataAccess.Repository
{
    public class WorkUnit : IWorkUnit
    {
        private readonly ApplicationDbContext _db;
        public IStoreRepository Store { get; private set; }

        public WorkUnit(ApplicationDbContext db)
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
