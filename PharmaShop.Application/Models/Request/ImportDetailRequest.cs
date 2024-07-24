namespace PharmaShop.Application.Models.Request
{
    public class ImportDetailRequest
    {
        public int ImportId {  get; set; }
        public int ProductId { get; set; }
        public string? BatchNumber { get; set; }
        public string? ManufactureDate { get; set; }
        public int Expiry { get; set; }
        public int Quantity { get; set; }
        public double Cost { get; set; }
    }
}
