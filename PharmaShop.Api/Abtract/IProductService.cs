using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;

namespace PharmaShop.Api.Abtract
{
    public interface IProductService
    {
        Task Add(ProductRequestModel data, List<IFormFile> images);
        Task<ProductForUpdateResponse> GetForUpdate(int id);
    }
}