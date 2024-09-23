using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PharmaShop.Application.Abtract;
using PharmaShop.Application.Models.Response;
using PharmaShop.Application.Models.Response.Report;
using PharmaShop.Domain.Abtract;
using PharmaShop.Domain.Entities;

namespace PharmaShop.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
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

        public async Task<ReportResponse<ProductReport>> GetProductStatisticsByDateAsync(string username, DateTime startDate, DateTime? endDate = null)
        {
            var user = await _userManager.FindByNameAsync(username) ?? throw new UnauthorizedAccessException($"Invalid username: {username}");

            var products = await _unitOfWork.Table<ProductInventory>().ToListAsync();
            var orderDetails = await _unitOfWork.Table<OrderDetail>()
                                                .Include(d => d.Order)
                                                .Include(d => d.Product)
                                                .Where(d => d.Order.OrderDate >= startDate && (endDate == null || d.Order.OrderDate <= endDate))
                                                .ToListAsync();

            var datas = orderDetails.GroupBy(d => new { d.ProductId, d.Product?.Name })
                                    .Select(g => new ProductReport
                                    {
                                        Id = g.Key.ProductId,
                                        Name = g.Key.Name ?? "Product",
                                        TotalPrice = g.Sum(d => d.Price * d.Quantity),
                                        SaleQuantity = g.Sum(d => d.Quantity),
                                        StoreQuantity = products.Where(i => i.ProductId == g.Key.ProductId).Sum(i => i.BigUnitQuantity),
                                    })
                                    .OrderByDescending(r => r.SaleQuantity)
                                    .ToList();

            return new ReportResponse<ProductReport>
            {
                From = startDate,
                To = endDate ?? DateTime.Now,
                Datas = datas,
                EmployeeName = user.FullName ?? "Employee",
                EmployeePhone = user.PhoneNumber ?? ""
            };
        }

        public async Task<ReportResponse<CustomerReport>> GetCustomerStatisticsByDateAsync(string username, DateTime startDate, DateTime? endDate = null)
        {
            var user = await _userManager.FindByNameAsync(username) ?? throw new UnauthorizedAccessException($"Invalid username: {username}");

            var customerAddresses = await _unitOfWork.Table<UserAddress>().Include(u => u.User).Where(u => u.IsActive == true).ToListAsync();

            var orders = await _unitOfWork.Table<Order>()
                                            .Include(o => o.Address)
                                            .Where(d => d.OrderDate >= startDate && (endDate == null || d.OrderDate <= endDate))
                                            .ToListAsync();

            var datas = orders.GroupBy(o => o.Address?.UserId)
                                .Select(group =>
                                {
                                    ApplicationUser? customer = customerAddresses.FirstOrDefault(c => c.UserId == group.Key)?.User;
                                    return new CustomerReport
                                    {
                                        Username = customer?.UserName ?? "Customer",
                                        CustomerName = customer?.FullName ?? "Customer",
                                        CustomerPhone = customer?.PhoneNumber ?? "",
                                        TotalOrders = group.Count(),
                                        TotalPrice = group.Sum(o => o.TotalPrice)
                                    };
                                })
                                .ToList();

            return new ReportResponse<CustomerReport>
            {
                From = startDate,
                To = endDate ?? DateTime.Now,
                Datas = datas,
                EmployeeName = user.FullName ?? "Employee",
                EmployeePhone = user.PhoneNumber ?? ""
            };
        }
    }
}
