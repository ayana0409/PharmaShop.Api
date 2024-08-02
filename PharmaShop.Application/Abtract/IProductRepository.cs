using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;
using PharmaShop.Infastructure.Entities;

namespace PharmaShop.Application.Abtract
{
    public interface IProductRepository
    {
        Task Add(Product product);
        void Edit(Product product);
        Task<IEnumerable<Product>> GetAllWithImage();
        Task<Product?> GetProductForSideByIdAsync(int id);
        Task<(IEnumerable<Product>, int)> GetProductForSidePanigationAsync(int pageIndex, int pageSize, string keyword, List<int> categories);
        Task<(IEnumerable<Product>, int)> GetProductPanigationAsync(int pageIndex, int pageSize, string keyword = "");
        Task<Product?> GetSigleAsync(int id);
        Task<Product?> GetSigleWithImageAsync(int id);
        Task Remove(int productId);
    }
}