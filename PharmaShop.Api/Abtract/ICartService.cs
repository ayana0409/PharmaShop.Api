using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;

namespace PharmaShop.Api.Abtract
{
    public interface ICartService
    {
        Task AddItemAsync(CartItemRequest request, string username);
        Task<int> GetItemsCount(string username);
        Task<TableResponse<CartItemResponse>> GetListItemsPaginationAsync(string username, TableRequest request);
    }
}