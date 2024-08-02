using Microsoft.VisualBasic;
using PharmaShop.Application.Abtract;
using PharmaShop.Infastructure.Data;
using PharmaShop.Infastructure.Entities;

namespace PharmaShop.Application.Repositorys
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }

        public async Task<IEnumerable<Category>> GetCategoriesPanigationAsync(int pageIndex, int pageSize, string keyword = "")
        {
            var query = await base.GetAllAsync();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(c => c.Name.Contains(keyword));
            }

            // Phân trang
            var categories = query
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();

            return categories;
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithoutParentAsync()
        {
            return await base.GetAllAsync(c => c.ParentId == null);
        }
        public async Task<IEnumerable<Category>> GetChildrenAsync(int CatregoryId)
        {
            return await base.GetAllAsync(c => c.ParentId ==  CatregoryId);
        }
    }
}
