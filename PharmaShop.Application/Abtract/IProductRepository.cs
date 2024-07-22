using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;
using PharmaShop.Infastructure.Entities;

namespace PharmaShop.Application.Abtract
{
    public interface IProductRepository
    {
        Task Add(Product product);
        void Edit(Product product);
        Task<(IEnumerable<Product>, int)> GetProductPanigationAsync(int pageIndex, int pageSize, string keyword = "");
        Task<Product?> GetSigleAsync(int id);
        Task Remove(int productId);
    }
}