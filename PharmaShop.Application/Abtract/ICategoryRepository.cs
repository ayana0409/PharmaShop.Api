using PharmaShop.Infastructure.Entities;

namespace PharmaShop.Application.Abtract
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetCategoriesPanigationAsync(int pageIndex, int pageSize, string keyword = "");
        Task<IEnumerable<Category>> GetCategoriesWithoutParentAsync();
        Task<IEnumerable<Category>> GetChildrenAsync(int CatregoryId);
    }
}