using PharmaShop.Domain.Entities;

namespace PharmaShop.Domain.Abtract
{
    public interface ICartItemRepository
    {
        Task<(IEnumerable<CartItem>, int)> GetPanigationByCartIdAsync(int cartId, int pageIndex, int pageSize);
    }
}