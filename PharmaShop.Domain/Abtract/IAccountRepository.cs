using PharmaShop.Domain.Entities;

namespace PharmaShop.Domain.Abtract
{
    public interface IAccountRepository
    {
        Task<(IEnumerable<ApplicationUser>, int)> GetPaginationAsync(int pageIndex, int pageSize, bool requireRole, string? keyword = null);
    }
}