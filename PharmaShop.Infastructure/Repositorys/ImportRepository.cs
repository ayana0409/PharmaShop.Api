using PharmaShop.Domain.Abtract;
using PharmaShop.Domain.Entities;
using PharmaShop.Application.Data;
using PharmaShop.Domain.Enum;

namespace PharmaShop.Application.Repositorys
{
    public class ImportRepository : GenericRepository<Import>, IImportRepository
    {
        public ImportRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }
        public async Task<(IEnumerable<Import>, int)> GetPanigationAsync(string supplierId, int pageIndex, int pageSize, string keyword)
        {
            var query = await base.GetAllAsync();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(i => i.SupplierId == supplierId && i.Id.ToString().Contains(keyword));
            }
            else
            {
                query = query.Where(i => i.SupplierId == supplierId);
            }

            var imports = query
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();

            int total = query.Count();

            return (imports, total);
        }


        public async Task<bool> CompleteImportAsync(int importId)
        {
            var import = await base.GetSigleAsync(i => i.Id ==  importId);

            if (import != null)
            {
                import.Status = Domain.Enum.StatusProcessing.Complete;

                base.Update(import);
                return true;
            }

            return false;
        }
    } 
}
