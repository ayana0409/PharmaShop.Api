using Microsoft.EntityFrameworkCore;
using PharmaShop.Application.Abtract;
using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;
using PharmaShop.Domain.Abtract;
using PharmaShop.Domain.Entities;

namespace PharmaShop.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CategoryTableResponse>> GetForTableAsync()
        {
            var data = await _unitOfWork.Table<Category>()
                                        .Where(c => c.IsAcvive == true)
                                        .Include(c => c.ParentCategory)
                                        .Select(c => new CategoryTableResponse
                                        {
                                            Id = c.Id,
                                            Name = c.Name ?? "Noname",
                                            ParentId = c.ParentCategory == null ? 0 : c.ParentId,
                                            ParentName = c.ParentCategory == null ? "" : c.ParentCategory.Name
                                        })
                                        .ToListAsync();

            return data;
        }

        public async Task AddAsync(CategoryRequest category)
        {
            try
            {
                await _unitOfWork.Table<Category>().AddAsync(new Category
                {
                    Name = category.Name,
                    ParentId = category.ParentId != 0 ? category.ParentId : null,
                    IsAcvive = true
                });
                
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Add category error", ex);
            }
        }

        public async Task UpdateAsync(int categoryId, CategoryRequest category)
        {
            try
            {
                Category oldCategory = await _unitOfWork.Table<Category>().FirstOrDefaultAsync(c => c.Id == categoryId)
                                        ?? throw new Exception(message: "Invalid category");

                oldCategory.Name = category.Name;
                oldCategory.ParentId = category.ParentId == 0 ? null : category.ParentId;
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Update category error", ex);
            }
        }

        public async Task DeleteAsync(int categoryId)
        {
            try
            {
                Category category = await _unitOfWork.Table<Category>().FirstOrDefaultAsync(c => c.Id == categoryId)
                            ?? throw new Exception(message: "Invalid category");

                category.IsAcvive = false;
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception ex) 
            { 
                throw new Exception("Delete category error", ex);
            }
        }
    }
}
