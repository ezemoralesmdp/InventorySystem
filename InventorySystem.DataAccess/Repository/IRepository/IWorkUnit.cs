using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.DataAccess.Repository.IRepository
{
    public interface IWorkUnit : IDisposable
    {
        IStoreRepository Store { get; }
        Task Save();
    }
}