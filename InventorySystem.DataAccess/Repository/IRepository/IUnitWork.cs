namespace InventorySystem.DataAccess.Repository.IRepository
{
    public interface IUnitWork : IDisposable
    {
        IStoreRepository Store { get; }
        ICategoryRepository Category { get; }
        IBrandRepository Brand { get; }
        IProductRepository Product { get; }
        IUserRepository User { get; }
        IStoreProductRepository StoreProduct { get; }
        IInventoryRepository Inventory { get; }
        IInventoryDetailRepository InventoryDetail { get; }
        IKardexInventoryRepository KardexInventory { get; }
        ICompanyRepository Company { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IOrderRepository Order { get; }
        IOrderDetailRepository OrderDetail { get; }
        Task Save();
    }
}