using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;

namespace PharmaShop.Application.Abtract
{
    public interface ICartService
    {
        Task AddItemAsync(CartItemRequest request, string username);
        Task DeleteCartItemAsync(int itemId);
        Task<int> GetItemsCount(string username);
        Task<TableResponse<CartItemResponse>> GetListItemsPaginationAsync(string username, TableRequest request);
        Task UpdateCartItemAsync(int itemId, int quantity);
    }
}