using PharmaShop.Domain.Enum;

namespace PharmaShop.Application.Models.Request
{
    public class OrderRequest
    {
        public int AddressId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Email { get; set; }
        public List<CartItemRequest> CartItems { get; set; } = [];
        public double TotalPrice { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
