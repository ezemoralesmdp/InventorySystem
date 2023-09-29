using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.DataAccess.Repository
{
    public class KardexInventoryRepository : Repository<KardexInventory>, IKardexInventoryRepository
    {
        private readonly ApplicationDbContext _db;

        public KardexInventoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task RegisterKardex(int storeProductId, string type, string detail, int stockPrevious, int amount, string userId)
        {
            var storeProduct = await _db.StoresProducts.Include(s => s.Product).FirstOrDefaultAsync(s => s.Id == storeProductId);

            if(type == "Input")
            {
                KardexInventory kardex = new();
                kardex.StoreProductId = storeProductId;
                kardex.Type = type;
                kardex.Detail = detail;
                kardex.PreviousStock = stockPrevious;
                kardex.Amount = amount;
                kardex.Cost = storeProduct.Product.Cost;
                kardex.Stock = stockPrevious + amount;
                kardex.Total = kardex.Stock * kardex.Cost;
                kardex.UserId = userId;
                kardex.RegistrationDate = DateTime.Now;

                await _db.KardexInventories.AddAsync(kardex);
                await _db.SaveChangesAsync();
            }
            if (type == "Output")
            {
                KardexInventory kardex = new();
                kardex.StoreProductId = storeProductId;
                kardex.Type = type;
                kardex.Detail = detail;
                kardex.PreviousStock = stockPrevious;
                kardex.Amount = amount;
                kardex.Cost = storeProduct.Product.Cost;
                kardex.Stock = stockPrevious - amount;
                kardex.Total = kardex.Stock * kardex.Cost;
                kardex.UserId = userId;
                kardex.RegistrationDate = DateTime.Now;

                await _db.KardexInventories.AddAsync(kardex);
                await _db.SaveChangesAsync();
            }
        }
    }
}