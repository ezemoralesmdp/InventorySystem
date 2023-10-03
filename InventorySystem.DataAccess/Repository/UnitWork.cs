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
        public IStoreProductRepository StoreProduct { get; private set; }
        public IInventoryRepository Inventory { get; private set; }
        public IInventoryDetailRepository InventoryDetail { get; private set; }
        public IKardexInventoryRepository KardexInventory { get; private set; }
        public ICompanyRepository Company { get; private set; }

        public UnitWork(ApplicationDbContext db)
        {
            _db = db;
            Store = new StoreRepository(_db);
            Category = new CategoryRepository(_db);
            Brand = new BrandRepository(_db);
            Product = new ProductRepository(_db);
            User = new UserRepository(_db);
            StoreProduct = new StoreProductRepository(_db);
            Inventory = new InventoryRepository(_db);
            InventoryDetail = new InventoryDetailRepository(_db);
            KardexInventory = new KardexInventoryRepository(_db);
            Company = new CompanyRepository(_db);
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