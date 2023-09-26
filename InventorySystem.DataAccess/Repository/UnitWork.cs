using InventorySystem.DataAccess.Repository.IRepository;

namespace InventorySystem.DataAccess.Repository
{
    public class UnitWork : IUnitWork
    {
        private readonly ApplicationDbContext _db;
        public IStoreRepository Store { get; private set; }
        public ICategoryRepository Category { get; private set; }
        public IBrandRepository Brand { get; private set; }
        public IProductRepository Product { get; private set; }
        public IUserRepository User { get; private set; }

        public UnitWork(ApplicationDbContext db)
        {
            _db = db;
            Store = new StoreRepository(_db);
            Category = new CategoryRepository(_db);
            Brand = new BrandRepository(_db);
            Product = new ProductRepository(_db);
            User = new UserRepository(_db);
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