namespace PharmaShop.Application.Models.Response.Report
{
    public class ReportResponse <T> where T : class
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public IEnumerable<T> Datas { get; set; } = [];
    }
}
