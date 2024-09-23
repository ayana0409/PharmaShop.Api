namespace PharmaShop.Application.Models.Response.Order
{
    public class OrderDetailResponse
    {
        public int ProductId { get; set; }
        public string? ImageUrl { get; set; } = string.Empty;
        public string? ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}
