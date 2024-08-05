using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;
using PharmaShop.Domain.Entities;
using PharmaShop.Domain.Enum;
using PharmaShop.Domain.Abtract;
namespace PharmaShop.Application.Services
{
    public class ImportService : IImportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ImportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ImportResponse> GetAsync(int id)
        {
            try
            {
                var result = await _unitOfWork.Table<Import>().FirstOrDefaultAsync(i => i.Id == id);

                if (result == null)
                {
                    throw new Exception("Invalid import");
                }

                await _unitOfWork.SaveChangeAsync();
                return new ImportResponse
                {
                    Id = result.Id,
                    Status = Enum.GetName(typeof(StatusProcessing), result.Status),
                    ImportDate = result.ImportDate,
                    TotalCost = result.TotalCost,
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<TableResponse<ImportResponse>> GetPanigationAsync(string supplierId, TableRequest request)
        {
            var (result, total) = await _unitOfWork.ImportRepository.GetPanigationAsync(supplierId, request.PageIndex, request.PageSize, request.Keyword ?? "");
            List<ImportResponse> datas = [];

            foreach (var item in result)
            {
                datas.Add(new ImportResponse
                {
                    Id = item.Id,
                    ImportDate = item.ImportDate,
                    TotalCost = item.TotalCost,
                    Status = Enum.GetName((StatusProcessing)item.Status),
                    
                });
            }

            return new TableResponse<ImportResponse>
            {
                PageSize = request.PageSize,
                Datas = datas,
                Total = total
            };
        }
        public async Task<int> CreateImportAsync(string supplierId)
        {
            try
            {
                var result = await _unitOfWork.Table<Import>().AddAsync(new Import
                {
                    ImportDate = DateTime.Now,
                    TotalCost = 0,
                    SupplierId = supplierId,
                    Status = StatusProcessing.New
                });

                await _unitOfWork.SaveChangeAsync();
                return result.Entity.Id;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteImportAsync(int importId)
        {
            try
            {
                await _unitOfWork.BeginTransaction();

                var import = await _unitOfWork.Table<Import>().FirstOrDefaultAsync(i => i.Id == importId) ?? throw new Exception("Invalid import.");

                if (import.Status == StatusProcessing.Complete)
                {
                    throw new Exception("Unable to delete completed import");
                }

                var importDetails = await _unitOfWork.Table<ImportDetail>().Where(d => d.ImportId == importId).ToListAsync();

                _unitOfWork.Table<ImportDetail>().RemoveRange(importDetails);

                _unitOfWork.Table<Import>().Remove(import);

                await _unitOfWork.SaveChangeAsync();
                await _unitOfWork.CommitTransaction();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackTransaction();
                throw new ApplicationException("Error deleting import.", ex);
            }
        }

        public async Task CompleteImportAsync(int importId)
        {
            try
            {
                await _unitOfWork.BeginTransaction();

                var result = await _unitOfWork.ImportRepository.CompleteImportAsync(importId);

                if (!result)
                {
                    throw new Exception("Can't completing import");
                }

                var importDetails = await _unitOfWork.Table<ImportDetail>().Where(d => d.ImportId == importId).ToListAsync();

                foreach (var detail in importDetails) 
                {
                    await _unitOfWork.Table<ProductInventory>().AddAsync(new ProductInventory
                    {
                        BatchNumber = detail.BatchNumber,
                        ProductId = detail.ProductId,
                        ManufactureDate = detail.ManufactureDate,
                        Expiry = detail.Expiry,
                        BigUnitQuantity = detail.Quantity,
                        MediumUnitQuantity = 0,
                        SmallUnitQuantity = 0,
                    });
                }

                await UpdateTotal(importId);

                await _unitOfWork.SaveChangeAsync();
                await _unitOfWork.CommitTransaction();
            }
            catch (Exception ex) 
            {
                await _unitOfWork.RollBackTransaction();
                throw new ApplicationException("Error.", ex);
            }
        }

        // Import detail
        public async Task<List<ImportDetailResponse>> GetDetailsByIdAsync(int importId)
        {
            try
            {
                return await _unitOfWork.Table<ImportDetail>()
                        .Where(d => d.ImportId == importId)
                        .Select(d => new ImportDetailResponse
                        {
                            ImportId = d.ImportId,
                            ProductId = d.ProductId,
                            ProductName = d.Product.Name,
                            BatchNumber = d.BatchNumber,
                            ManufactureDate = d.ManufactureDate.ToString("yyyy-MM-dd"),
                            Expiry = d.Expiry,
                            Quantity = d.Quantity,
                            Cost = d.Cost,
                        }).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task AddDetailsAsync(ImportDetailRequest request)
        {
            try
            {
                await _unitOfWork.BeginTransaction();

                var result = await _unitOfWork.Table<ImportDetail>().FirstOrDefaultAsync(d => d.ImportId == request.ImportId && d.ProductId == request.ProductId);

                if (result == null)
                {
                    await _unitOfWork.ImportDetailRepository.AddDetailAsync(new ImportDetail
                    {
                        ImportId = request.ImportId,
                        ProductId = request.ProductId,
                        BatchNumber = "",
                        ManufactureDate = DateTime.Now,
                        Expiry = 0,
                        Quantity = 0,
                        Cost = 0,
                    });
                }
                else
                {
                    result.Quantity++;
                    _unitOfWork.Table<ImportDetail>().Update(result);
                }

                await _unitOfWork.SaveChangeAsync();
                await _unitOfWork.CommitTransaction();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackTransaction();
                throw new ApplicationException("Error adding import details.", ex);
            }
        }

        public async Task UpdateDetailAsync(ImportDetailRequest request)
        {
            try
            {
                await _unitOfWork.BeginTransaction();

                var detail = await _unitOfWork.Table<ImportDetail>()
                            .FirstOrDefaultAsync(d => d.ImportId == request.ImportId 
                                                    && d.ProductId == request.ProductId);

                if (detail == null)
                {
                    throw new Exception("Invalid details");
                }

                detail.BatchNumber = request.BatchNumber;
                detail.ManufactureDate = DateTime.Parse(request.ManufactureDate ?? DateTime.MinValue.ToString());
                detail.Expiry = request.Expiry;
                detail.Quantity = request.Quantity;
                detail.Cost = request.Cost;

                _unitOfWork.Table<ImportDetail>().Update(detail);

                await UpdateTotal(detail.ImportId);

                await _unitOfWork.SaveChangeAsync();
                await _unitOfWork.CommitTransaction();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackTransaction();
                throw new ApplicationException(ex.Message);
            }
        }
        public async Task RemoveDetails(int importId, int productId)
        {
            try
            {
                await _unitOfWork.BeginTransaction();

                var detail = await _unitOfWork.Table<ImportDetail>()
                                .FirstOrDefaultAsync(d => d.ImportId == importId
                                                    && d.ProductId == productId);

                if (detail == null)
                {
                    throw new Exception("Invalid details");
                }

                _unitOfWork.Table<ImportDetail>().Remove(detail);

                await UpdateTotal(importId);

                await _unitOfWork.SaveChangeAsync();
                await _unitOfWork.CommitTransaction();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackTransaction();
                throw new ApplicationException(ex.Message);
            }
        }

        private async Task UpdateTotal(int importId)
        {
            var import = await _unitOfWork.Table<Import>().FirstOrDefaultAsync(i => i.Id == importId);

            if (import != null)
            {
                import.TotalCost = await _unitOfWork.Table<ImportDetail>()
                            .Where(d => d.ImportId == importId)
                            .SumAsync(d => (d.Quantity * d.Cost));

               _unitOfWork.Table<Import>().Update(import);
            }

            
            await _unitOfWork.SaveChangeAsync();
        }
    }
}
