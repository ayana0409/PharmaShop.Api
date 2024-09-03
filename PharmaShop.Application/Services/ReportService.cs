using Microsoft.EntityFrameworkCore;
using PharmaShop.Application.Abtract;
using PharmaShop.Application.Models.Response.Report;
using PharmaShop.Domain.Abtract;
using PharmaShop.Domain.Entities;

namespace PharmaShop.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<RevenueRecordResponse>> CalculateWeeklyRevenue()
        {
            var startDate = DateTime.Now.Date.AddDays(-6);
            var endDate = DateTime.Now.Date;

            var weeklyRevenue = await _unitOfWork.Table<Order>()
                .Where(o => o.OrderDate.Date >= startDate.Date && o.OrderDate.Date <= endDate.Date)
                .GroupBy(t => t.OrderDate.Date)
                .Select(g => new RevenueRecordResponse
                {
                    Date = g.Key,
                    Revenue = g.Sum(t => t.TotalPrice)
                })
                .ToListAsync();

            return weeklyRevenue;
        }
    }
}
