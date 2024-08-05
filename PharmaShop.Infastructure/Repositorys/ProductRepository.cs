using Microsoft.EntityFrameworkCore;
using PharmaShop.Domain.Abtract;
using PharmaShop.Domain.Entities;
using PharmaShop.Application.Data;

namespace PharmaShop.Application.Repositorys
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ProductRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IQueryable<Product> GetProductsWithDetails()
        {
            return _applicationDbContext.Set<Product>()
                .Include(p => p.Category)
                .Include(p => p.Details)
                .Include(p => p.Images)
                .Include(p => p.ProductInventorys);
        }

        public async Task<(IEnumerable<Product>, int)> GetProductPanigationAsync(int pageIndex, int pageSize, string keyword)
        {
            var query = await base.GetAllAsync();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(c => c.IsActive == true &&
                                        c.Name != null && 
                                        (c.Name.Contains(keyword, StringComparison.CurrentCultureIgnoreCase)
                                        || c.Id.ToString().Contains(keyword, StringComparison.CurrentCultureIgnoreCase)));
            }
            else
            {
                query = query.Where(p => p.IsActive == true);
            }

            var products = query
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();

            int total = query.Count();

            return (products, total);
        }

        public async Task<(IEnumerable<Product>, int)> GetProductForSidePanigationAsync(int pageIndex, int pageSize, string keyword, List<int> categories)
        {
            var query = GetProductsWithDetails();


            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(c => (categories.Count <= 0 || categories.Contains(c.CategoryId))
                                        && c.Name != null
                                        && (c.Name.Contains(keyword, StringComparison.CurrentCultureIgnoreCase)
                                        || c.Id.ToString().Contains(keyword, StringComparison.CurrentCultureIgnoreCase)));
            }
            else
            {
                query = query.Where(c => (categories.Count <= 0 || categories.Contains(c.CategoryId)));
            }

            var products = await query
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            int total = await query.CountAsync();

            return (products, total);
        }

        public async Task<Product?> GetProductForSideByIdAsync(int id)
        {
            var query = GetProductsWithDetails();
            return await query.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Product?> GetSigleAsync(int id)
        {
            return await base.GetSigleAsync(p => p.Id == id);
        }

        public async Task<Product?> GetSigleWithImageAsync(int id)
        {
            return await _applicationDbContext.Set<Product>()
                .Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetAllWithImage()
        {
            return await _applicationDbContext.Set<Product>()
                .Include(p => p.Images).Where(p => p.IsActive == true).ToListAsync();
        }

        public async Task Add(Product product)
        {
            await base.CreateAsync(product);
        }
        public void Edit(Product product)
        {
            base.Update(product);
        }
        public async Task Remove(int productId)
        {
            var product = await base.GetSigleAsync(p => p.Id == productId);

            if (product != null)
            {
                product.IsActive = false;
                base.Update(product);
            }
        }
    }
}
