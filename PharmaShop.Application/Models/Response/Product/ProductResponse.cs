using PharmaShop.Application.Models.Request;

namespace PharmaShop.Application.Models.Response
{
    public class ProductResponse
    {
        public string? Name { get; set; }
        public string? Brand { get; set; }
        public int CategoryId { get; set; }
        public string? Description { get; set; }
        public double Taxing { get; set; }
        public string? Unit { get; set; }
        public string? Packaging { get; set; }
        public int BigUnit { get; set; }
        public int MediumUnit { get; set; }
        public int SmallUnit { get; set; }
        public double BigUnitPrice { get; set; }
        public double MediumUnitPrice { get; set; }
        public double SmallUnitPrice { get; set; }

        public bool RequirePrescription { get; set; }

        public List<ProductDetailRequest> Details { get; set; } = [];
        public List<string> Images { get; set; } = [];
    }
}
