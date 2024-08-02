using PharmaShop.Infastructure.Entities;

namespace PharmaShop.Application.Abtract
{
    public interface IImportDetailRepository
    {
        Task AddDetailAsync(ImportDetail importDetail);
    }
}