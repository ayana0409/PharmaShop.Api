using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
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

                if (images != null & images.Count >= 0)
                {
                    var productImages = new List<Image>();

                    foreach (var file in images)
                    {
                        if (file.Length > 0)
                        {
                            var photoUrl = await _cloudinaryService.UploadPhotoAsync(file);
                            productImages.Add(new Image
                            {
                                ProductId = result.Entity.Id,
                                Path = photoUrl,
                                IsActive = true
                            });

                        }
                    }
                    await _unitOfWork.Table<Image>().AddRangeAsync(productImages);
                }
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

            List<ProductDetailRequest> details = [];
            foreach (var item in _unitOfWork.Table<ProductDetail>().Where(d => d.ProductId == id).ToList()) 
            { 
                details.Add(new ProductDetailRequest { 
                    Id = item.Id,
                    Content = item.Content, 
                    Name = item.Name,
                });
            }

            List<string> Images = [];
            foreach (var item in _unitOfWork.Table<Image>().Where(d => d.ProductId == id).ToList())
            {
                Images.Add(item.Path);
            }

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
    }
}
