namespace PharmaShop.Application.Models.Response
{
    public class ImportResponse
    {
        public int Id { get; set; }
        public DateTime ImportDate { get; set; }
        public double TotalCost { get; set; }
        public string? Status { get; set; }
    }
}
