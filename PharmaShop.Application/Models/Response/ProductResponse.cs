namespace PharmaShop.Application.Models.Response
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Packaging { get; set; } = "";
        public string Brand { get; set; } = "";
        public Double Price { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}
