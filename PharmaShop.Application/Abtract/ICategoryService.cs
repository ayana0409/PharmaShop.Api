using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;

namespace PharmaShop.Application.Abtract
{
    public interface ICategoryService
    {
        Task AddAsync(CategoryRequest category);
        Task DeleteAsync(int categoryId);
        Task<IEnumerable<CategoryTableResponse>> GetForTableAsync();
        Task UpdateAsync(int categoryId, CategoryRequest category);
    }
}