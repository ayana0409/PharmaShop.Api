using Microsoft.AspNetCore.Http;
using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;
using PharmaShop.Application.Models.Response.Product;

namespace PharmaShop.Application.Abtract
{
    public interface IProductService
    {
        Task Add(ProductRequest data, List<IFormFile> images);
        Task Delete(int productId);
        Task<ProductResponse> GetForUpdate(int id);
        Task<TableResponse<ProductSummary>> GetPagigation(TableRequest request);
        Task Update(int id, ProductRequest data, List<IFormFile> images, List<string> imageUrl);
    }
}