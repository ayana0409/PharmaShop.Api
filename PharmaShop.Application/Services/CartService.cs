using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PharmaShop.Application.Abtract;
using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;
using PharmaShop.Domain.Entities;
using PharmaShop.Domain.Abtract;

namespace PharmaShop.Application.Services
{
    public class CartService : ICartService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public CartService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> GetItemsCount(string username)
        {
            var user = await _userManager.FindByNameAsync(username) ?? throw new Exception("Invalid user");

            var cart = await _unitOfWork.Table<Cart>().FirstOrDefaultAsync(c => c.UserId == user.Id) ?? await CreateCartAsync(user.Id);

            var result = await _unitOfWork.Table<CartItem>().Where(i => i.CartId == cart.Id).CountAsync();

            return result;
        }

        public async Task<TableResponse<CartItemResponse>> GetListItemsPaginationAsync(string username, TableRequest request)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username) ?? throw new Exception("Invalid user");

                var cart = await _unitOfWork.Table<Cart>().FirstOrDefaultAsync(c => c.UserId == user.Id) ?? await CreateCartAsync(user.Id);

                var (items, total) = await _unitOfWork.CartItemRepository.GetPanigationByCartIdAsync(cart.Id, request.PageIndex, request.PageSize);

                var datas = new List<CartItemResponse>();

                var products = await _unitOfWork.ProductRepository.GetAllWithImage();
                foreach (var item in items)
                {
                    var product  = products.FirstOrDefault(p => p.Id == item.ProductId);
                    if (product != null)
                    {
                        datas.Add(new CartItemResponse
                        {
                            Id = item.Id,
                            ProductName = product.Name,
                            Price = product.BigUnitPrice,
                            Quantity = item.Quantity,
                            ImageUrl = product.Images?.FirstOrDefault()?.Path ?? ""
                        });
                    }
                }

                return new TableResponse<CartItemResponse>
                {
                    PageSize = request.PageSize,
                    Total = total,
                    Datas = datas
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddItemAsync(CartItemRequest request, string username)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username) ?? throw new Exception("Invalid user");

                var cart = await _unitOfWork.Table<Cart>().FirstOrDefaultAsync(c => c.UserId == user.Id) ?? await CreateCartAsync(user.Id);

                var existItem = await _unitOfWork.Table<CartItem>().FirstOrDefaultAsync(i => i.CartId == cart.Id && i.ProductId == request.ProductId);

                if (existItem == null)
                {
                    var newItem = new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = request.ProductId,
                        Quantity = request.Quantity
                    };

                    await _unitOfWork.Table<CartItem>().AddAsync(newItem);
                }
                else
                {
                    existItem.Quantity += request.Quantity;

                    _unitOfWork.Table<CartItem>().Update(existItem);
                }

                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding item to cart.", ex);
            }
        }

        public async Task UpdateCartItemAsync(int itemId, int quantity)
        {
            try
            {
                var existItem = await _unitOfWork.Table<CartItem>().FirstOrDefaultAsync(i => i.Id == itemId);

                if (existItem == null)
                {
                    return;
                }

                if (quantity > 0)
                {
                    existItem.Quantity = quantity;
                    _unitOfWork.Table<CartItem>().Update(existItem);
                }
                else
                {
                    _unitOfWork.Table<CartItem>().Remove(existItem);
                }

                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteCartItemAsync(int itemId)
        {
            try
            {
                var existItem = await _unitOfWork.Table<CartItem>().FirstOrDefaultAsync(i => i.Id == itemId);

                if (existItem == null)
                {
                    return;
                }

                _unitOfWork.Table<CartItem>().Remove(existItem);

                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task<Cart> CreateCartAsync(string userId)
        {
            try
            {
                var newCart = new Cart
                {
                    UserId = userId,
                    CreatedDate = DateTime.Now
                };

                await _unitOfWork.Table<Cart>().AddAsync(newCart);
                await _unitOfWork.SaveChangeAsync();

                return newCart;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in cart creation:", ex);
            }
        }
    }
}
