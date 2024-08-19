using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;

namespace PharmaShop.Application.Abtract
{
    public interface ICartService
    {
        Task<bool> AddItemAsync(CartItemRequest request, string username);
        Task DeleteCartItemAsync(int itemId);
        Task DeleteCartItemRangeAsync(List<int> listCartItemIds);
        Task<int> GetItemsCount(string username);
        Task<TableResponse<CartItemResponse>> GetListItemsPaginationAsync(string username, TableRequest request);
        Task UpdateCartItemAsync(int itemId, int quantity);
    }
}