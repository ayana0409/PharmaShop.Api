using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaShop.Application.Abtract;
using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;
using PharmaShop.Application.Models.Response.Order;
using System.Security.Claims;

namespace PharmaShop.Application.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;

        public UserController(IUserService userService, IOrderService orderService)
        {
            _userService = userService;
            _orderService = orderService;
        }
        [HttpGet("getuserinfo")]
        public async Task<ActionResult<CustomerInfoResponse>> UserInfo()
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (username == null)
                {
                    return Unauthorized();
                }

                var user = await _userService.GetUserByUserNameAsync(username);

                CustomerInfoResponse response = new();

                response.FullName = user.FullName ?? "No name";
                response.Type = user.Type == null ? "No type" : user.Type.Name;
                response.Point = user.Point;
                response.Discount = user.Type?.Discount ?? 0;
                response.MaxDiscount = user.Type?.MaxDiscount ?? 0;

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("address")]
        public async Task<ActionResult<UserAddressResponse>> GetUserAddress()
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (username == null)
                {
                    return Unauthorized();
                }

                var result = await _userService.GetAddressByUsernameAsync(username);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("address/{addressId}")]
        public async Task<IActionResult> DeleteUserAddress(int addressId)
        {
            try
            {
                var result = await _userService.RemoveAddressAsync(addressId);

                if (!result)
                {
                    return NotFound(addressId);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("orders/{id}")]
        public async Task<ActionResult<OrderResponse>> GetOrder(int id)
        {
            try
            {
                OrderResponse order = await _orderService.GetById(id);

                return Ok(order);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("orders")]
        public async Task<ActionResult<TableResponse<OrderResponse>>> UserOrders(TableRequest request)
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (username == null)
                {
                    return Unauthorized();
                }

                var user = await _userService.GetUserByUserNameAsync(username);

                var result = await _orderService.GetOrdersByUsernamePaginationAsync(request, username);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
