namespace PharmaShop.Application.Models.Request
{
    public class CartItemRequest
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
