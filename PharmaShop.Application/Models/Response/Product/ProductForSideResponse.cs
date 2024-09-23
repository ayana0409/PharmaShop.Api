namespace PharmaShop.Application.Models.Response
{
    public class ProductForSideResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Packaging { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public double Price { get; set; }
        public bool RequirePrescription { get; set; }
        public bool IsActive { get; set; }

        public List<string> ImagesUrl { get; set; } = [];
    }
}
