using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models;

namespace InventorySystem.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Category category)
        {
            var categoryDB = _db.Categories.FirstOrDefault(s => s.Id == category.Id);

            if(categoryDB != null)
            {
                categoryDB.Name = category.Name;
                categoryDB.Description = category.Description;
                categoryDB.State = category.State;
                _db.SaveChanges();
            }
        }
    }
}