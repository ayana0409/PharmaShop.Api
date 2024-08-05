using PharmaShop.Domain.Abtract;
using PharmaShop.Domain.Entities;
using PharmaShop.Application.Data;

namespace PharmaShop.Application.Repositorys
{
    public class ImportDetailRepository : GenericRepository<ImportDetail>, IImportDetailRepository
    {
        public ImportDetailRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }

        public async Task AddDetailAsync(ImportDetail importDetail)
        {
            await base.CreateAsync(importDetail);
        }
    }
}
