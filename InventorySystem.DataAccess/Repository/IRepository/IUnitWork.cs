namespace InventorySystem.DataAccess.Repository.IRepository
{
    public interface IUnitWork : IDisposable
    {
        IStoreRepository Store { get; }
        ICategoryRepository Category { get; }
        Task Save();
    }
}