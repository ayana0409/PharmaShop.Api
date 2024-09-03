namespace PharmaShop.Application.Models.Response
{
    public class TypeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Discount { get; set; }
        public double MaxDiscount { get; set; }
        public int Point { get; set; }
    }
}
