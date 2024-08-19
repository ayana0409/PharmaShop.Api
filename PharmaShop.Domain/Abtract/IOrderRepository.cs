using PharmaShop.Domain.Entities;

namespace PharmaShop.Domain.Abtract
{
    public interface IOrderRepository
    {
        Task<(IEnumerable<Order>, int)> GetPanigationAsync(int pageIndex, int pageSize, string? userId = null, string? keyword = null);
    }
}