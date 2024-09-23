using PharmaShop.Application.Models.Response;
using PharmaShop.Application.Models.Response.Report;

namespace PharmaShop.Application.Abtract
{
    public interface IReportService
    {
        Task<List<RevenueRecordResponse>> CalculateWeeklyRevenue();
        Task<ReportResponse<CustomerReport>> GetCustomerStatisticsByDateAsync(string username, DateTime startDate, DateTime? endDate = null);
        Task<ReportResponse<ProductReport>> GetProductStatisticsByDateAsync(string username, DateTime startDate, DateTime? endDate = null);
    }
}