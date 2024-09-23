using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaShop.Application.Abtract;
using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response.Order;
using PharmaShop.Domain.Enum;
using System.Security.Claims;

namespace PharmaShop.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IPayPalService _payPalService;

        public OrdersController(IUserService userService,
                                IOrderService orderService,
                                ICartService cartService,
                                IPayPalService payPalService)
        {
            _userService = userService;
            _orderService = orderService;
            _cartService = cartService;
            _payPalService = payPalService;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponse>> Get(int id)
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

        [HttpGet]
        public async Task<ActionResult<List<OrderResponse>>> GetPagination([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 5, [FromQuery] string keyword = "")
        {
            try
            {
                var result = await _orderService.GetOrdersByUsernamePaginationAsync(new TableRequest
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    Keyword = keyword
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Details/{id}")]
        public async Task<ActionResult<List<OrderDetailResponse>>> GetDetail(int id)
        {
            try
            {
                var datas = await _orderService.GetDetailsAsync(id);
                return datas;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
        {
            try
            {
                if (Enum.IsDefined(typeof(StatusProcessing), request.Status))
                {
                    await _orderService.UpdateStatusAsync(id, (StatusProcessing)request.Status);
                    return NoContent();
                }

                return BadRequest("Invalid status value.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] OrderRequest request)
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized();
                }

                if (request.PaymentMethod == PaymentMethod.PayPal)
                {
                    if (!_payPalService.ProcessPayment(request))
                    {
                        return BadRequest();
                    }
                }

                await _orderService.CreateAsync(username, request);

                await _userService.AddUserPointByTotalPriceAsync(username, request.TotalPrice);

                List<int> cartItemIds = request.CartItems.Select(x => x.Id).ToList();

                await _cartService.DeleteCartItemRangeAsync(cartItemIds);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _orderService.CancelOrder(id);
                return result ? Ok(id) : NotFound(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); 
            }

        }
    }
}
