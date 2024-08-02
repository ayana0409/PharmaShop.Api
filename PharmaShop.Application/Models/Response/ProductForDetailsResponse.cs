namespace PharmaShop.Application.Models.Response
{
    public class ProductForDetailsResponse
    {
        public int Id {  get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string Packaging { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public double Price {  get; set; }
        public List<string>? Images { get; set; }
        public List<ProductDetailForSideResponse>? Details { get; set; }
        public bool RequirePrescription { get; set; }
        public bool IsActive { get; set; }

    }
}
