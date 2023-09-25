using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventorySystem.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
            var productDB = _db.Products.FirstOrDefault(s => s.Id == product.Id);

            if(productDB != null)
            {
                if(productDB.ImageUrl != null)
                {
                    productDB.ImageUrl = product.ImageUrl;
                }

                productDB.SerialNumber = product.SerialNumber;
                productDB.Description = product.Description;
                productDB.Price = product.Price;
                productDB.Cost = product.Cost;
                productDB.CategoryId = product.CategoryId;
                productDB.BrandId = product.BrandId;
                productDB.FatherId = product.FatherId;
                productDB.State = product.State;

                _db.SaveChanges();
            }
        }

        public IEnumerable<SelectListItem> GetAllDropDownList(string obj)
        {
            if(obj == "Category")
            {
                return _db.Categories.Where(c => c.State == true).Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });
            }

            if (obj == "Brand")
            {
                return _db.Brands.Where(c => c.State == true).Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });
            }

            if (obj == "Product")
            {
                return _db.Products.Where(c => c.State == true).Select(c => new SelectListItem
                {
                    Text = c.Description,
                    Value = c.Id.ToString()
                });
            }

            return null;
        }
    }
}