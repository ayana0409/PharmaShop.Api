using PharmaShop.Domain.Entities;

namespace PharmaShop.Domain.Abtract
{
    public interface IImageRepository
    {
        Task<IEnumerable<Image>> GetByProductIdAsync(int productId);
        void Remove(Image image);
    }
}