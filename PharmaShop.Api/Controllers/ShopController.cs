using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;
using PharmaShop.Application.Abtract;

namespace PharmaShop.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly IShopService _shopService;

        public ShopController(IShopService shopService)
        {
            _shopService = shopService;
        }

        [HttpGet("navbar")]
        public async Task<IEnumerable<NavbarResponse>> GetNavbar()
        {
             return await _shopService.GetNavbar();
        }

        [HttpPost("products/panigation")]
        public async Task<ActionResult<TableResponse<ProductForSideResponse>>> GetProductPanigation([FromBody] ProductForSideRequest request)
        {
            try
            {
                var result = await _shopService.GetProductForSidePanigationAsync(request);
                return Ok(result);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductForDetailsResponse>> Get(int id)
        {
            try
            {
                var result = await _shopService.GetProductForSideById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
