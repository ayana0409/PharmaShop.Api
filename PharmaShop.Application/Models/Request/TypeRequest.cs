namespace PharmaShop.Application.Models.Request
{
    public class TypeRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Discount { get; set; }
        public double MaxDiscount { get; set; }
        public int Point { get; set; }
    }
}
