using Microsoft.EntityFrameworkCore;

namespace PharmaShop.Application.Abtract
{
    public interface IUnitOfWork
    {
        Task BeginTransaction();
        Task CommitTransaction();
        void Dispose();
        Task RollBackTransaction();
        Task SaveChangeAsync();
        DbSet<T> Table<T>() where T : class;
    }
}