using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;

namespace PharmaShop.Application.Abtract
{
    public interface ITypeService
    {
        Task CreateAsync(TypeRequest typeRequest);
        Task<List<TypeResponse>> GetAllAsync();
        Task<TypeResponse> GetByIdAsync(int typeId);
        Task RemoveAsync(int typeId);
        Task UpdateAsync(TypeRequest typeRequest);
    }
}