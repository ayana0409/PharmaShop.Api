using Microsoft.EntityFrameworkCore;
using PharmaShop.Application.Abtract;
using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;
using PharmaShop.Domain.Entities;
using PharmaShop.Domain.Abtract;
using Microsoft.AspNetCore.Http;

namespace PharmaShop.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;

        public ProductService(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<TableResponse<ProductResponse>> GetPagigation(TableRequest request)
        {
            var (result, total) = await _unitOfWork.ProductRepository
                .GetProductPanigationAsync(request.PageIndex, request.PageSize, request.Keyword ?? "");
            List<ProductResponse> datas = [];
            var categories = await _unitOfWork.Table<Category>().ToListAsync();

            foreach (var item in result)
            {
                datas.Add(new ProductResponse
                {
                    Id = item.Id,
                    Name = item.Name ?? "",
                    Brand = item.Brand ?? "",
                    Packaging = item.Packaging ?? "",
                    Price = item.BigUnitPrice,
                    CategoryId = item.CategoryId,
                    CategoryName = categories.FirstOrDefault(c => c.Id == item.CategoryId)?.Name
                });
            }

            return new TableResponse<ProductResponse>
            {
                PageSize = request.PageSize,
                Datas = datas,
                Total = total
            };
        }

        public async Task Add(ProductRequest data, List<IFormFile> images)
        {
            Product product = new()
            {
                Name = data.Name,
                Brand = data.Brand,
                Description = data.Description,
                CategoryId = data.CategoryId,
                Taxing = data.Taxing,
                RequirePrescription = data.RequirePrescription,
                Packaging = data.Packaging,
                Unit = data.Unit,
                BigUnit = data.BigUnit,
                BigUnitPrice = data.BigUnitPrice,
                MediumUnit = data.MediumUnit,
                MediumUnitPrice = data.MediumUnit > 0 ? data.MediumUnitPrice : 0,
                SmallUnit = data.SmallUnit,
                SmallUnitPrice = data.SmallUnit > 0 ? data.SmallUnitPrice : 0,
                IsActive = true
            };

            await _unitOfWork.BeginTransaction();
            try
            {
                var result = await _unitOfWork.Table<Product>().AddAsync(product);

                await _unitOfWork.SaveChangeAsync();

                if (data.Details.Count > 0) 
                {
                    List<ProductDetail> details = [];

                    foreach (var item in data.Details)
                    {
                        details.Add(new ProductDetail
                        {
                            ProductId = result.Entity.Id,
                            Name = item.Name,
                            Content = item.Content,

                        });
                    }
                    await _unitOfWork.Table<ProductDetail>().AddRangeAsync(details);
                }

                await AddImageList(result.Entity.Id, images);

                await _unitOfWork.SaveChangeAsync();

                await _unitOfWork.CommitTransaction();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackTransaction();
                throw new Exception(message: ex.Message);
            }
        }

        public async Task Update(int id, ProductRequest data, List<IFormFile> images, List<string> imageUrl)
        {
            try
            {
                await _unitOfWork.BeginTransaction();
                Product? product = await _unitOfWork.ProductRepository.GetSigleAsync(id);

                if (product == null)
                {
                    throw new Exception(message: "Invalid product");
                }

                product.Name = data.Name;
                product.Brand = data.Brand;
                product.Description = data.Description;
                product.CategoryId = data.CategoryId;
                product.Taxing = data.Taxing;
                product.RequirePrescription = data.RequirePrescription;
                product.Packaging = data.Packaging;
                product.Unit = data.Unit;
                product.BigUnit = data.BigUnit;
                product.BigUnitPrice = data.BigUnitPrice;
                product.MediumUnit = data.MediumUnit;
                product.MediumUnitPrice = data.MediumUnit > 0 ? data.MediumUnitPrice : 0;
                product.SmallUnit = data.SmallUnit;
                product.SmallUnitPrice = data.SmallUnit > 0 ? data.SmallUnitPrice : 0;
                product.IsActive = true;
                
                _unitOfWork.ProductRepository.Edit(product);

                await _unitOfWork.SaveChangeAsync();

                var details = await _unitOfWork.ProductDetailRepository.GetByProductId(id);

                List<ProductDetail> insertDetailList = data.Details
                                                    .Where(d => d.Id == 0)
                                                    .Select(d => {
                                                        return new ProductDetail
                                                        {
                                                            Name = d.Name,
                                                            Content = d.Content,
                                                            ProductId = id
                                                        };
                                                    }).ToList();

                await _unitOfWork.Table<ProductDetail>().AddRangeAsync(insertDetailList);

                foreach (var item in details)
                {
                    var newData = data.Details.FirstOrDefault(d => d.Id == item.Id);
                    if (newData != null)
                    {
                        item.Content = newData.Content;
                        item.Name = newData.Name;
                        _unitOfWork.ProductDetailRepository.Edit(item);
                    }
                    else
                    {
                        _unitOfWork.ProductDetailRepository.Remove(item);
                    }
                }

                var productImages = await _unitOfWork.ImageRepository.GetByProductIdAsync(id);

                var validImageUrls = imageUrl.Where(i => i.Contains("http://res.cloudinary.com/")).ToList();

                var deleteTasks = productImages
                                    .Where(image => image.Path != null && !validImageUrls.Contains(image.Path))
                                    .Select(image => {
                                        _unitOfWork.ImageRepository.Remove(image);
                                        return _cloudinaryService.DeleteAsync(image.Path);
                                    })
                                    .ToArray();

                await Task.WhenAll(deleteTasks);

                var insertImage = images.Where(i => i.FileName != "blob").ToList();

                await AddImageList(id, insertImage);

                await _unitOfWork.SaveChangeAsync();
                await _unitOfWork.CommitTransaction();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackTransaction();
                throw new Exception(message: ex.Message);
            }
        }

        public async Task Delete(int productId)
        {
            try
            {
                await _unitOfWork.BeginTransaction();

                var productImages = await _unitOfWork.ImageRepository.GetByProductIdAsync(productId);

                var deleteTasks = productImages
                                    .Select(image => {
                                        _unitOfWork.ImageRepository.Remove(image);
                                        return _cloudinaryService.DeleteAsync(image.Path);
                                    })
                                    .ToArray();

                await Task.WhenAll(deleteTasks);

                await _unitOfWork.ProductRepository.Remove(productId);

                await _unitOfWork.SaveChangeAsync();
                await _unitOfWork.CommitTransaction();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackTransaction();
                throw new Exception(message: ex.Message);
            }
        }

        public async Task<ProductForUpdateResponse> GetForUpdate(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetSigleAsync(id);

            if (product == null)
            {
                throw new Exception(message: "Invalid product");
            }

            var listDetails = _unitOfWork.Table<ProductDetail>().Where(d => d.ProductId == id);

            List<ProductDetailRequest> details = listDetails != null 
                                ? listDetails.Select(detail => new ProductDetailRequest
                                                        {
                                                            Id = detail.Id,
                                                            Content = detail.Content,
                                                            Name = detail.Name
                                                        }).ToList() : [];

            var listImages = _unitOfWork.Table<Image>().Where(d => d.ProductId == id);

            List<string> Images = listImages != null ? listImages.Select(d => d.Path).ToList() : [];

            return new ProductForUpdateResponse
            {
                Name = product.Name,
                Brand = product.Brand,
                CategoryId = product.CategoryId,
                Taxing = product.Taxing,
                Unit = product.Unit,
                Packaging = product.Packaging,
                Description = product.Description,
                BigUnit = product.BigUnit,
                MediumUnit = product.MediumUnit,
                SmallUnit = product.SmallUnit,
                BigUnitPrice = product.BigUnitPrice,
                MediumUnitPrice = product.MediumUnitPrice,
                SmallUnitPrice = product.SmallUnitPrice,
                RequirePrescription = product.RequirePrescription,
                Details = details,
                Images = Images
            };
        }

        private async Task AddImageList(int productId, List<IFormFile>? images)
        {
            if (images != null && images.Count >= 0)
            {
                var productImages = new List<Image>();

                foreach (var file in images)
                {
                    if (file.Length > 0)
                    {
                        var photoUrl = await _cloudinaryService.UploadPhotoAsync(file);
                        productImages.Add(new Image
                        {
                            ProductId = productId,
                            Path = photoUrl,
                            IsActive = true
                        });

                    }
                }
                await _unitOfWork.Table<Image>().AddRangeAsync(productImages);
            }
        }
    }
}
