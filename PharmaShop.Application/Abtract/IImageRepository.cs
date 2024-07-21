using PharmaShop.Infastructure.Entities;

namespace PharmaShop.Application.Abtract
{
    public interface IImageRepository
    {
        Task<IEnumerable<Image>> GetByProductIdAsync(int productId);
        void Remove(Image image);
    }
}