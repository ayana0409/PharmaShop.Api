﻿using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;
using PharmaShop.Application.Models.Response.Order;
using PharmaShop.Domain.Enum;

namespace PharmaShop.Application.Abtract
{
    public interface IOrderService
    {
        Task<bool> CancelOrder(int orderId);
        Task CreateAsync(string username, OrderRequest orderRequest);
        Task<OrderResponse> GetById(int orderId);
        Task<List<OrderDetailResponse>> GetDetailsAsync(int id);
        Task<TableResponse<OrderResponse>> GetOrdersByUsernamePaginationAsync(TableRequest request, string? username = null);
        Task UpdateStatusAsync(int orderId, StatusProcessing status);
    }
}