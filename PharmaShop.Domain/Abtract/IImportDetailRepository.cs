using PharmaShop.Domain.Entities;

namespace PharmaShop.Domain.Abtract
{
    public interface IImportDetailRepository
    {
        Task AddDetailAsync(ImportDetail importDetail);
    }
}