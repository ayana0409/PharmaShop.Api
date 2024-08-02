using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PharmaShop.Api.Services;
using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;

namespace PharmaShop.Api.Controllers
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
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("getnavbar")]
        public async Task<IEnumerable<NavbarResponse>> GetNavbar()
        {
             return await _shopService.GetNavbar();
        }

        [HttpPost("getproductpanigation")]
        public async Task<TableResponse<ProductForSideResponse>> GetProductPanigation([FromForm] string request)
        {
            var datas = JsonConvert.DeserializeObject<ProductForSideRequest>(request);

            return datas == null ? throw new Exception("Request is null") : await _shopService.GetProductForSidePanigationAsync(datas);
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

        [HttpPost]
        public void Post([FromBody] string value)
        {
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
