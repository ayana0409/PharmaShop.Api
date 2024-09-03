using PharmaShop.Application.Models.Response.Report;

namespace PharmaShop.Application.Abtract
{
    public interface IReportService
    {
        Task<List<RevenueRecordResponse>> CalculateWeeklyRevenue();
    }
}