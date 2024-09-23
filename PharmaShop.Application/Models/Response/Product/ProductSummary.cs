namespace PharmaShop.Application.Models.Response.Product
{
    public class ProductSummary
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Packaging { get; set; } = "";
        public string Brand { get; set; } = "";
        public double Price { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}
