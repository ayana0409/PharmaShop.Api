namespace PharmaShop.Application.Models.Response
{
    public class CustomerInfoResponse
    {
        public string? FullName { get; set; }
        public double Discount { get; set; }
        public double MaxDiscount { get; set; }
        public int Point {  get; set; }
        public string? Type { get; set; }
    }
}
