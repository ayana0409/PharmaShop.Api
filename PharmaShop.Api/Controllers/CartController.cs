using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using PharmaShop.Api.Abtract;
using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;
using System.Security.Claims;

namespace PharmaShop.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("getitemscount")]
        public async Task<int> GetItemsCount()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (username == null)
            {
                return 0;
            }
            return await _cartService.GetItemsCount(username);
        }

        [HttpPost("additem")]
        public async Task<ActionResult> AddItem([FromBody] CartItemRequest request)
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (username == null)
                {
                    return Unauthorized();
                }

                await _cartService.AddItemAsync(request, username);

                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("getitems")]
        public async Task<ActionResult<TableResponse<CartItemResponse>>> GetItems([FromForm] string request)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (username == null)
            {
                return Unauthorized();
            }

            var data = JsonConvert.DeserializeObject<TableRequest>(request);

            if (data == null)
            {
                return BadRequest();
            }

            return await _cartService.GetListItemsPaginationAsync(username, data);
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
