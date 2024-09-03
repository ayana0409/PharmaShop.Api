using Microsoft.EntityFrameworkCore;
using PharmaShop.Application.Abtract;
using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;
using PharmaShop.Domain.Abtract;
using PharmaShop.Domain.Entities;

namespace PharmaShop.Application.Services
{
    public class TypeService : ITypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TypeResponse>> GetAllAsync()
        {
            var result = await _unitOfWork.Table<UserType>()
                                            .Where(t => t.IsActive == true)
                                            .OrderBy(t => t.Point)
                                            .Select(t => new TypeResponse
                                            {
                                                Id = t.Id,
                                                Name = t.Name ?? "",
                                                Discount = t.Discount,
                                                MaxDiscount = t.MaxDiscount,
                                                Point = t.Point,
                                            })
                                            .ToListAsync();

            return result;
        }

        public async Task<TypeResponse> GetByIdAsync(int typeId)
        {
            var type = await _unitOfWork.Table<UserType>()
                                            .OrderBy(t => t.Point)
                                            .FirstOrDefaultAsync(t => t.Id == typeId)
                                            ?? throw new KeyNotFoundException($"Not found Type id {typeId}");

            TypeResponse result = new()
            {
                Id = type.Id,
                Name = type.Name ?? "",
                Discount = type.Discount,
                MaxDiscount = type.MaxDiscount,
                Point = type.Point,
            };

            return result;
        }

        public async Task CreateAsync(TypeRequest typeRequest)
        {
            await _unitOfWork.BeginTransaction();

            if (string.IsNullOrEmpty(typeRequest.Name))
            {
                throw new Exception("Name is not empty");
            }

            UserType type = new()
            {
                Name = typeRequest.Name,
                Discount = typeRequest.Discount,
                MaxDiscount = typeRequest.MaxDiscount,
                Point = typeRequest.Point,
                IsActive = true,
            };

            await _unitOfWork.Table<UserType>().AddAsync(type);

            await _unitOfWork.SaveChangeAsync();
            await _unitOfWork.CommitTransaction();
        }

        public async Task UpdateAsync(TypeRequest typeRequest)
        {
            UserType type = await _unitOfWork.Table<UserType>()
                                                .FirstOrDefaultAsync(t => t.Id == typeRequest.Id)
                                                ?? throw new KeyNotFoundException($"Not found Type id {typeRequest.Id}");
            
            if (string.IsNullOrEmpty(typeRequest.Name))
            {
                throw new Exception("Name is not empty");
            }

            await _unitOfWork.BeginTransaction();

            type.Name = typeRequest.Name;
            type.Discount = typeRequest.Discount;
            type.MaxDiscount = typeRequest.MaxDiscount;
            type.Point = typeRequest.Point;

            _unitOfWork.Table<UserType>().Update(type);

            await _unitOfWork.SaveChangeAsync();
            await _unitOfWork.CommitTransaction();
        }

        public async Task RemoveAsync(int typeId)
        {
            UserType type = await _unitOfWork.Table<UserType>()
                                                .FirstOrDefaultAsync(t => t.Id == typeId)
                                                ?? throw new KeyNotFoundException($"Not found Type id {typeId}");

            await _unitOfWork.BeginTransaction();

            type.IsActive = false;

            _unitOfWork.Table<UserType>().Update(type);

            await _unitOfWork.SaveChangeAsync();
            await _unitOfWork.CommitTransaction();
        }

    }
}
