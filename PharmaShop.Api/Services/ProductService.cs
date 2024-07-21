using Microsoft.EntityFrameworkCore;
using PharmaShop.Api.Abtract;
using PharmaShop.Application.Abtract;
using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;
using PharmaShop.Infastructure.Entities;

namespace PharmaShop.Api.Services
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

        public async Task<TableResponseModel<ProductResponseModel>> GetPanigation(TableRequestModel request)
        {
            var (result, total) = await _unitOfWork.ProductRepository.GetProductPanigationAsync(request.PageIndex, request.PageSize, request.Keyword = string.Empty);
            List<ProductResponseModel> datas = [];
            var categories = await _unitOfWork.Table<Category>().ToListAsync();

            foreach (var item in result)
            {
                datas.Add(new ProductResponseModel
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

            return new TableResponseModel<ProductResponseModel>
            {
                PageSize = request.PageSize,
                Datas = datas,
                Total = total
            };
        }

        public async Task Add(ProductRequestModel data, List<IFormFile> images)
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

        public async Task Update(int id, ProductRequestModel data, List<IFormFile> images, List<string> imageUrl)
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

                foreach(var item in insertDetailList)
                {
                    await _unitOfWork.ProductDetailRepository.AddAsync(item);
                }

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

                List<string> containImageUrl = imageUrl.Where(i => i.Contains("http://res.cloudinary.com/")).ToList();

                var productImages = await _unitOfWork.ImageRepository.GetByProductIdAsync(id);

                foreach (var image in productImages)
                {
                    var result = containImageUrl.FirstOrDefault(ci => ci == image.Path);
                    if (result == null) 
                    {
                        var uri = new Uri(image.Path ?? "");
                        var segments = uri.Segments;
                        if (segments.Length >= 3)
                        {
                            var publicIdWithFormat = segments[^1];
                            var publicId = publicIdWithFormat.Substring(0, publicIdWithFormat.LastIndexOf('.'));
                             
                            await _cloudinaryService.DeleteAsync(publicId);

                            _unitOfWork.ImageRepository.Remove(image);
                        }
                    }
                }
                
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

        public async Task<ProductForUpdateResponse> GetForUpdate(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetSigleAsync(id);

            if (product == null)
            {
                throw new Exception(message: "Invalid product");
            }

            var listDetails = _unitOfWork.Table<ProductDetail>().Where(d => d.ProductId == id);

            List<ProductDetailRequest> details = listDetails != null ? listDetails.Select(detail => new ProductDetailRequest
            {
                Id = detail.Id,
                Content = detail.Content,
                Name = detail.Name
            }).ToList() : [];

            var listImages = _unitOfWork.Table<Image>().Where(d => d.ProductId == id);

            List<string?> Images = listImages != null ? listImages.Select(d => d.Path).ToList() : [];

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
