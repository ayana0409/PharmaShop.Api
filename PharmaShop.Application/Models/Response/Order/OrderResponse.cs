namespace PharmaShop.Application.Models.Response.Order
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalPrice { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int TotalItems { get; set; }
        public List<OrderDetailResponse> Details { get; set; } = [];
    }
}
