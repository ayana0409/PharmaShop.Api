namespace PharmaShop.Application.Models.Request
{
    public class CartItemUpdateRequest
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
    }
}
