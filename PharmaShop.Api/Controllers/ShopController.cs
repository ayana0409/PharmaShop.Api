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

        [HttpGet("homeProduct")]
        public async Task<ActionResult<IEnumerable<HomeProductResponse>>> GetHomeProduct()
        {
            try
            {
                var result = await _shopService.GetHomeProductListAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("products")]
        public async Task<ActionResult<TableResponse<ProductForSideResponse>>> GetProductPanigation([FromQuery] int categoryId = 0, [FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 5, [FromQuery] string keyword = "")
        {
            try
            {
                var result = await _shopService.GetProductForSidePanigationAsync(new ProductForSideRequest
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    Keyword = keyword,
                    CategoryId = categoryId
                });
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
