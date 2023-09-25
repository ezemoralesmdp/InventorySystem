namespace InventorySystem.DataAccess.Repository.IRepository
{
    public interface IUnitWork : IDisposable
    {
        IStoreRepository Store { get; }
        ICategoryRepository Category { get; }
        IBrandRepository Brand { get; }
        IProductRepository Product { get; }
        Task Save();
    }
}