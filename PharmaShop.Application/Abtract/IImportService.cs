
using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;

namespace PharmaShop.Application.Services
{
    public interface IImportService
    {
        Task AddDetailsAsync(ImportDetailRequest request);
        Task CompleteImportAsync(int importId);
        Task<int> CreateImportAsync(string supplierId);
        Task DeleteImportAsync(int importId);
        Task<ImportResponse> GetAsync(int id);
        Task<List<ImportDetailResponse>> GetDetailsByIdAsync(int importId);
        Task<TableResponse<ImportResponse>> GetPanigationAsync(string supplierId, TableRequest request);
        Task RemoveDetails(int importId, int productId);
        Task UpdateDetailAsync(ImportDetailRequest request);
    }
}