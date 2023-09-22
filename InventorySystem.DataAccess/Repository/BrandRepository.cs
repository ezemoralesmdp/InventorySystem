using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models;

namespace InventorySystem.DataAccess.Repository
{
    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        private readonly ApplicationDbContext _db;

        public BrandRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Brand brand)
        {
            var brandDB = _db.Brands.FirstOrDefault(s => s.Id == brand.Id);

            if(brandDB != null)
            {
                brandDB.Name = brand.Name;
                brandDB.Description = brand.Description;
                brandDB.State = brand.State;
                _db.SaveChanges();
            }
        }
    }
}