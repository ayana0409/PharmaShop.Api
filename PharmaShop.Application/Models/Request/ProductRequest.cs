namespace PharmaShop.Application.Models.Request
{
    public class ProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string? Description { get; set; }
        public double Taxing { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string Packaging { get; set; } = string.Empty;
        public int BigUnit { get; set; }
        public int MediumUnit { get; set; }
        public int SmallUnit { get; set; }
        public double BigUnitPrice { get; set; }
        public double MediumUnitPrice { get; set; }
        public double SmallUnitPrice { get; set; }

        public bool RequirePrescription { get; set; }

        public List<ProductDetailRequest> Details { get; set; } = [];
    }
}
