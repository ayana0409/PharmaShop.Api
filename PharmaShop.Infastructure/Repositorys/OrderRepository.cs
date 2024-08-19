using Microsoft.EntityFrameworkCore;
using PharmaShop.Application.Data;
using PharmaShop.Application.Repositorys;
using PharmaShop.Domain.Abtract;
using PharmaShop.Domain.Entities;

namespace PharmaShop.Infastructure.Repositorys
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public OrderRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<(IEnumerable<Order>, int)> GetPanigationAsync(int pageIndex, int pageSize, string? userId = null, string? keyword = null)
        {
            var query = await _applicationDbContext.Orders
                                                    .Include(o => o.Address)
                                                    .Include(o => o.Details)
                                                    .OrderByDescending(o => o.Id)
                                                    .Where(o => (userId == null || o.Address.UserId == userId) &&
                                                                (string.IsNullOrEmpty(keyword) ||
                                                                 o.Id.ToString().Contains(keyword) ||
                                                                 o.Address.PhoneNumber.Contains(keyword) ||
                                                                 o.Address.FullName.Contains(keyword))
                                                    ).ToListAsync();

            var products = query
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();

            int total = query.Count();

            return (products, total);
        }
    }
}
