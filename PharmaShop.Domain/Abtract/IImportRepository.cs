using PharmaShop.Domain.Entities;

namespace PharmaShop.Domain.Abtract
{
    public interface IImportRepository
    {
        Task<(IEnumerable<Import>, int)> GetPanigationAsync(string supplierId, int pageIndex, int pageSize, string keyword);
        Task<bool> CompleteImportAsync(int importId);
    }
}