using Microsoft.EntityFrameworkCore;

namespace PharmaShop.Application.Abtract
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        IProductDetailRepository ProductDetailRepository { get; }
        IImageRepository ImageRepository { get; }

        Task BeginTransaction();
        Task CommitTransaction();
        void Dispose();
        Task RollBackTransaction();
        Task SaveChangeAsync();
        DbSet<T> Table<T>() where T : class;
    }
}