using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;

namespace PharmaShop.Application.Abtract
{
    public interface IShopService
    {
        Task<List<NavbarResponse>> GetNavbar();
        Task<ProductForDetailsResponse> GetProductForSideById(int id);
        Task<TableResponse<ProductForSideResponse>> GetProductForSidePanigationAsync(ProductForSideRequest request);
    }
}