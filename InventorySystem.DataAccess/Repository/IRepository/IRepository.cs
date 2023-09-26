using InventorySystem.Models.Specifications;
using System.Linq.Expressions;

namespace InventorySystem.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<T> Get(int id);

        Task<IEnumerable<T>> GetAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null,
            bool isTracking = true
        );

        PagedList<T> GetAllPaginated(
            Parameters parameters, 
            Expression<Func<T, bool>> filter = null, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, 
            string includeProperties = null,
            bool isTracking = true
        );

        Task<T> GetFirst(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = null,
            bool isTracking = true
        );

        Task Add(T entity);

        //Los metodos REMOVE no pueden ser asincronos
        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entities);
    }
}