using PharmaShop.Application.Abtract;
using PharmaShop.Infastructure.Data;
using PharmaShop.Infastructure.Entities;

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
