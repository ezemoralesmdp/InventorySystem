using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models;

namespace InventorySystem.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _db;

        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Company company)
        {
            var companyDB = _db.Companies.FirstOrDefault(s => s.Id == company.Id);

            if(companyDB != null)
            {
                companyDB.Name = company.Name;
                companyDB.Description = company.Description;
                companyDB.Country = company.Country;
                companyDB.City = company.City;
                companyDB.Address = company.Address;
                companyDB.Telephone = company.Telephone;
                companyDB.StoreSellId = company.StoreSellId;
                companyDB.UpdatedById = company.UpdatedById;
                companyDB.UpdateDate = company.UpdateDate;
                _db.SaveChanges();
            }
        }
    }
}