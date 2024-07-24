namespace PharmaShop.Application.Models.Response
{
    public class ImportDetailResponse
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? BatchNumber { get; set; }
        public string? ManufactureDate { get; set; }
        public int Expiry { get; set; }
        public int Quantity { get; set; }
        public double Cost { get; set; }
    }
}
