using PharmaShop.Application.Abtract;
using PharmaShop.Infastructure.Data;
using PharmaShop.Infastructure.Entities;
using ZstdSharp.Unsafe;

namespace PharmaShop.Application.Repositorys
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
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

        public async Task<Product?> GetSigleAsync(int id)
        {
            return await base.GetSigleAsync(p => p.Id == id);
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
