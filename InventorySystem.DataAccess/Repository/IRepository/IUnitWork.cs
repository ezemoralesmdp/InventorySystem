namespace InventorySystem.DataAccess.Repository.IRepository
{
    public interface IUnitWork : IDisposable
    {
        IStoreRepository Store { get; }
        Task Save();
    }
}