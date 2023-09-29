using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventorySystem.DataAccess.Repository
{
    public class InventoryRepository : Repository<Inventory>, IInventoryRepository
    {
        private readonly ApplicationDbContext _db;

        public InventoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Inventory inventory)
        {
            var inventoryDB = _db.Inventories.FirstOrDefault(s => s.Id == inventory.Id);

            if(inventoryDB != null)
            {
                inventoryDB.StoreId = inventory.StoreId;
                inventoryDB.FinalDate = inventory.FinalDate;
                inventoryDB.State = inventory.State;
                _db.SaveChanges();
            }
        }

        public IEnumerable<SelectListItem> GetAllDropDownList(string obj)
        {
            if(obj == "Store")
            {
                return _db.Stores.Where(s => s.State == true).Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString()
                });
            }

            return null;
        }
    }
}