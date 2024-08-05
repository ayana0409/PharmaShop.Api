using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PharmaShop.Domain.Abtract;
using PharmaShop.Application.Data;

namespace PharmaShop.Application.Repositorys
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private IDbContextTransaction _dbContextTransaction;
        private ICategoryRepository? _categoryRepository;
        private IProductRepository? _productRepository;
        private IProductDetailRepository? _productDetailRepository;
        private IImageRepository? _imageRepository;
        private IImportRepository? _importRepository;
        private IImportDetailRepository? _importDetailRepository;
        private ICartItemRepository? _cartItemRepository;
        public UnitOfWork(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public DbSet<T> Table<T>() where T : class => _applicationDbContext.Set<T>();

        // Repository
        public ICategoryRepository CategoryRepository => _categoryRepository ??= new CategoryRepository(_applicationDbContext);
        public IProductRepository ProductRepository => _productRepository ??= new ProductRepository(_applicationDbContext);
        public IProductDetailRepository ProductDetailRepository => _productDetailRepository ??= new ProductDetailRepository(_applicationDbContext);
        public IImageRepository ImageRepository => _imageRepository ??= new ImageRepository(_applicationDbContext);
        public IImportRepository ImportRepository => _importRepository ??= new ImportRepository(_applicationDbContext);
        public IImportDetailRepository ImportDetailRepository => _importDetailRepository ??= new ImportDetailRepository(_applicationDbContext);
        public ICartItemRepository CartItemRepository => _cartItemRepository ??= new CartItemRepository(_applicationDbContext);
        //

        public async Task BeginTransaction()
        {
            _dbContextTransaction = await _applicationDbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitTransaction()
        {
            await _dbContextTransaction?.CommitAsync();
        }

        public async Task RollBackTransaction()
        {
            await _dbContextTransaction?.RollbackAsync();
        }

        public async Task SaveChangeAsync()
        {
            await _applicationDbContext?.SaveChangesAsync();
        }
        public void Dispose()
        {
            if (_applicationDbContext != null)
            {
                _applicationDbContext.Dispose();
            }
        }
    }
}
