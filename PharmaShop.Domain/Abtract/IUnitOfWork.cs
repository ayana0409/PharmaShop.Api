using Microsoft.EntityFrameworkCore;

namespace PharmaShop.Domain.Abtract
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        IProductDetailRepository ProductDetailRepository { get; }
        IImageRepository ImageRepository { get; }
        IImportRepository ImportRepository { get; }
        IImportDetailRepository ImportDetailRepository { get; }
        ICartItemRepository CartItemRepository { get; }
        IOrderRepository OrderRepository { get; }
        IAccountRepository AccountRepository { get; }

        Task BeginTransaction();
        Task CommitTransaction();
        void Dispose();
        Task RollBackTransaction();
        Task SaveChangeAsync();
        DbSet<T> Table<T>() where T : class;
    }
}