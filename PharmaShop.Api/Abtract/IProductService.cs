using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;

namespace PharmaShop.Api.Abtract
{
    public interface IProductService
    {
        Task Add(ProductRequestModel data, List<IFormFile> images);
        Task<ProductForUpdateResponse> GetForUpdate(int id);
        Task<TableResponseModel<ProductResponseModel>> GetPanigation(TableRequestModel request);
        Task Update(int id, ProductRequestModel data, List<IFormFile> images, List<string> imageUrl);
    }
}