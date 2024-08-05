using Microsoft.EntityFrameworkCore;
using PharmaShop.Application.Models;
using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;
using PharmaShop.Domain.Entities;
using PharmaShop.Domain.Abtract;

namespace PharmaShop.Application.Services
{
    public class ShopService : IShopService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShopService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<NavbarResponse>> GetNavbar()
        {
            var categories = await _unitOfWork.Table<Category>()
                                              .Include(c => c.Categories)
                                              .ToListAsync();

            var navbar = categories.Where(c => c.ParentId == null).ToList();

            List<NavbarResponse> result = new List<NavbarResponse>();

            foreach (var category in navbar)
            {
                var children = category.Categories;

                if (children != null)
                {
                    result.Add(new NavbarResponse
                    {
                        Id = category.Id,
                        Title = category.Name ?? "Category",
                        Items = children.Select(c => new NavbarChild { Label = c.Name ?? "Child name", Href = "products/category/" +c.Id.ToString() }).ToList(),
                    });
                }
            }

            return result;
        }

        public async Task<TableResponse<ProductForSideResponse>> GetProductForSidePanigationAsync(ProductForSideRequest request) 
        {
            var categories = request.CategoryId != 0 ? GetChildCategoryIdsByParentId(request.CategoryId) : [];
            var (datas, total) = await _unitOfWork.ProductRepository
                                    .GetProductForSidePanigationAsync(request.PageIndex, request.PageSize, request.Keyword, categories);

            List<ProductForSideResponse> products = [];

            foreach (var item in datas) {
                products.Add(new ProductForSideResponse
                {
                    Id = item.Id,
                    Name = item.Name ?? "",
                    Description = item.Description ?? "",
                    Packaging = item.Packaging ?? "",
                    Quantity = item.ProductInventorys != null ? item.ProductInventorys.Sum(i => i.BigUnitQuantity) : 0,
                    RequirePrescription = item.RequirePrescription,
                    Price = item.BigUnitPrice,
                    ImagesUrl = item.Images != null ? item.Images.Select(i => i.Path).ToList() : [],
                    IsActive = item.IsActive,
                });
            }

            return new TableResponse<ProductForSideResponse>
            {
                Datas = products,
                PageSize = request.PageSize,
                Total = total
            };
        }

        public async Task<ProductForDetailsResponse> GetProductForSideById(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetProductForSideByIdAsync(id);

            if (product == null)
            {
                throw new Exception("Invalid product");
            }

            var images = product.Images?.Select(product => product.Path).ToList() ?? [];
            var details = product.Details?.Select(p => new ProductDetailForSideResponse
            {
                Name = p.Name ?? "",
                Content = p.Content ?? ""
            }).ToList() ?? [];

            return new ProductForDetailsResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description ?? "",
                Unit = product.Unit,
                Packaging = product.Packaging,
                Brand = product.Brand,
                Category = product.Category?.Name ?? "Unknown",
                Quantity = product.ProductInventorys != null ? product.ProductInventorys.Sum(i => i.BigUnitQuantity) : 0,
                Price = product.BigUnitPrice,
                Images = images,
                Details = details,
                IsActive = product.IsActive,
                RequirePrescription = product.RequirePrescription,
            };
        }

        public List<int> GetAllChildCategoryIds(int parentId, List<Category> allCategories)
        {
            var childCategoryIds = new List<int>();

            void GetChildCategories(int id)
            {
                var childCategories = allCategories.Where(c => c.ParentId == id).ToList();
                foreach (var category in childCategories)
                {
                    childCategoryIds.Add(category.Id);
                    GetChildCategories(category.Id);
                }
            }

            GetChildCategories(parentId);
            return childCategoryIds;
        }

        public List<int> GetChildCategoryIdsByParentId(int parentCategoryId)
        {
            var allCategories = _unitOfWork.Table<Category>().ToList();

            var childCategoryIds = GetAllChildCategoryIds(parentCategoryId, allCategories);

            childCategoryIds.Add(parentCategoryId);

            return childCategoryIds;
        }
    }
}
