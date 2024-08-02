using PharmaShop.Infastructure.Entities;

namespace PharmaShop.Application.Abtract
{
    public interface ICartItemRepository
    {
        Task<(IEnumerable<CartItem>, int)> GetPanigationByCartIdAsync(int cartId, int pageIndex, int pageSize);
    }
}