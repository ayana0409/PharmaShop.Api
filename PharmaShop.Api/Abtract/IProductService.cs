﻿using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;

namespace PharmaShop.Api.Abtract
{
    public interface IProductService
    {
        Task Add(ProductRequest data, List<IFormFile> images);
        Task Delete(int productId);
        Task<ProductForUpdateResponse> GetForUpdate(int id);
        Task<TableResponse<ProductResponse>> GetPanigation(TableRequest request);
        Task Update(int id, ProductRequest data, List<IFormFile> images, List<string> imageUrl);
    }
}