using PharmaShop.Application.Models.Request;

namespace PharmaShop.Application.Abtract
{
    public interface IPayPalService
    {
        bool ProcessPayment(OrderRequest request);
    }
}