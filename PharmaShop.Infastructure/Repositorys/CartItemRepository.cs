using PharmaShop.Domain.Abtract;
using PharmaShop.Domain.Entities;
using PharmaShop.Application.Data;

namespace PharmaShop.Application.Repositorys
{
    public class CartItemRepository : GenericRepository<CartItem>, ICartItemRepository
    {
        public CartItemRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {

        }

        public async Task<(IEnumerable<CartItem>, int)> GetPanigationByCartIdAsync(int cartId, int pageIndex, int pageSize)
        {
            var query = await base.GetAllAsync(i => i.CartId == cartId);

            var products = query
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();

            int total = query.Count();

            return (products, total);
        }
    }
}
