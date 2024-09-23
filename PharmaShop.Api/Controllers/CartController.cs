using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaShop.Application.Abtract;
using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;
using System.Security.Claims;

namespace PharmaShop.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("items/count")]
        public async Task<int> GetItemsCount()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (username == null)
            {
                return 0;
            }
            return await _cartService.GetItemsCount(username);
        }

        [Authorize]
        [HttpPost("items")]
        public async Task<ActionResult> AddItem([FromBody] CartItemRequest request)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (username == null)
            {
                return Unauthorized();
            }

            try
            {
                var result = await _cartService.AddItemAsync(request, username);

                return Created("Add new item:", result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<TableResponse<CartItemResponse>>> GetItems([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 5, [FromQuery] string keyword = "")
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (username == null)
            {
                return Unauthorized();
            }

            return await _cartService.GetListItemsPaginationAsync(username, new TableRequest
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Keyword = keyword
            });
        }

        [Authorize]
        [HttpPut("items/{itemId}")]
        public async Task<ActionResult> UpdateCartItem(int itemId, [FromBody] CartItemUpdateRequest request)
        {
            if (itemId != request.ItemId)
            {
                return BadRequest("Item ID mismatch.");
            }

            try
            {
                await _cartService.UpdateCartItemAsync(itemId, request.Quantity);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [Authorize]
        [HttpDelete("items/{itemId}")]
        public async Task<IActionResult> DeleteCartItem(int itemId)
        {
            try
            {
                await _cartService.DeleteCartItemAsync(itemId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
