using InventorySystem.Models;

namespace InventorySystem.DataAccess.Repository.IRepository
{
    public interface IStoreRepository : IRepository<Store>
    {
        void Update(Store store); //Actualizar se maneja de manera individual porque cada entidad tiene sus propias propiedades
    }
}