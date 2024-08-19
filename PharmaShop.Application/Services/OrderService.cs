using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PharmaShop.Application.Abtract;
using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;
using PharmaShop.Domain.Abtract;
using PharmaShop.Domain.Entities;
using PharmaShop.Domain.Enum;
using Order = PharmaShop.Domain.Entities.Order;

namespace PharmaShop.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<TableResponse<OrderResponse>> GetOrdersByUsernamePaginationAsync(TableRequest request, string? username = null)
        {
            string? userId = null;

            if (!string.IsNullOrEmpty(username))
            {
                var user = await _userManager.FindByNameAsync(username) ?? throw new Exception("Unauthorize");

                userId = user.Id;
            }

            var (orders, total) = await _unitOfWork.OrderRepository.GetPanigationAsync(request.PageIndex, request.PageSize, userId, request.Keyword);

            List<OrderResponse> datas = orders.Select(o =>
            {

                return new OrderResponse
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    PaymentMethod = Enum.GetName(typeof(PaymentMethod), o.PaymentMethod) ?? "COD",
                    Status = Enum.GetName(typeof(StatusProcessing), o.Status) ?? "Error",
                    TotalPrice = o.TotalPrice,
                    TotalItems = o.Details?.Count ?? 0,
                    Phone = o.Address?.PhoneNumber ?? string.Empty,
                    FullName = o.Address?.FullName ?? string.Empty,
                };
            }).ToList();

            return new TableResponse<OrderResponse> 
            {
                PageSize = request.PageSize,
                Total = total,
                Datas = datas
            };
        }

        public async Task CreateAsync(string username, OrderRequest orderRequest)
        {
            var user = await _userManager.FindByNameAsync(username) ?? throw new Exception("Unauthorize");
            var products = await _unitOfWork.Table<Product>().Where(p => p.IsActive == true && 
                                                orderRequest.CartItems.Select(c => c.ProductId).Contains(p.Id)).ToListAsync();

            try
            {
                await _unitOfWork.BeginTransaction();

                if (orderRequest.AddressId == 0)
                {
                    var address = new UserAddress
                    {
                        UserId = user.Id,
                        FullName = orderRequest.FullName,
                        PhoneNumber = orderRequest.PhoneNumber,
                        Address = orderRequest.Address,
                        Email = orderRequest.Email,
                        IsActive = true,
                    };

                    await _unitOfWork.Table<UserAddress>().AddAsync(address);
                    await _unitOfWork.SaveChangeAsync();

                    orderRequest.AddressId = address.Id;
                }
                var order = new Order
                {
                    UserAddressId = orderRequest.AddressId,
                    OrderDate = DateTime.Now,
                    PaymentMethod = orderRequest.PaymentMethod,
                    Status = StatusProcessing.New,
                    TotalPrice = orderRequest.TotalPrice
                };

                await _unitOfWork.Table<Order>().AddAsync(order);
                await _unitOfWork.SaveChangeAsync();

                List<OrderDetail> details = [];

                foreach (var item in orderRequest.CartItems)
                {
                    details.Add(new OrderDetail
                    {
                        OrderId = order.Id,
                        Quantity = item.Quantity,
                        IsUnit = false,
                        ProductId = item.ProductId,
                        Price = products.FirstOrDefault(p => p.Id == item.ProductId)?.BigUnitPrice ?? 0,
                    });
                }
                await _unitOfWork.Table<OrderDetail>().AddRangeAsync(details);

                await _unitOfWork.SaveChangeAsync();
                await _unitOfWork.CommitTransaction();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackTransaction();
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateStatusAsync(int orderId, StatusProcessing status)
        {
            try
            {
                var order = await _unitOfWork.Table<Order>().FirstOrDefaultAsync(o => o.Id == orderId) ?? throw new KeyNotFoundException($"Order with ID {orderId} not found.");

                order.Status = status;

                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<OrderDetailResponse>> GetDetailsAsync(int id)
        {
            var details = await _unitOfWork.Table<OrderDetail>()
                                            .Where(d => d.OrderId == id)
                                            .Include(d => d.Product)
                                            .Select(d =>
                                                new OrderDetailResponse
                                                {
                                                    Quantity = d.Quantity,
                                                    Price = d.Price,
                                                    ProductId = d.ProductId,
                                                    ProductName = d.Product.Name,
                                                    ImageUrl = d.Product.Images != null ? d.Product.Images.First().Path : ""
                                                }
                                            ).ToListAsync();

            return details;
        }

        public async Task<bool> CancelOrder(int orderId)
        {
            Order? order = await _unitOfWork.Table<Order>().FirstOrDefaultAsync(o => o.Id == orderId);

            if (order != null) 
            { 
                order.Status = StatusProcessing.Cancel;

                _unitOfWork.Table<Order>().Update(order);

                await _unitOfWork.SaveChangeAsync();
                return true;
            }

            return false;
        }
    }
}
