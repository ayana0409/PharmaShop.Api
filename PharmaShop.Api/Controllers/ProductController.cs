using Microsoft.AspNetCore.Mvc;
using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;
using Newtonsoft.Json;
using PharmaShop.Application.Abtract;
using Microsoft.AspNetCore.Authorization;

namespace PharmaShop.Application.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/<ProductController>
        [HttpGet("getall")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("getforupdate")]
        public async Task<ActionResult<ProductForUpdateResponse>> GetForUpdate(int id)
        {
            try
            {
                var result = await _productService.GetForUpdate(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("pagination")]
        public async Task<ActionResult<TableResponse<ProductResponse>>> GetPanigation([FromBody] TableRequest request)
        {

            try
            {
                var result = await _productService.GetPanigation(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("add")]
        public async Task<ActionResult> Add([FromForm] string data, List<IFormFile> images)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<ProductRequest>(data);

                if (model == null)
                {
                    throw new Exception(message: "model error");
                }

                CheckModelValid(model);

                await _productService.Add(model, images);

                return Ok(new {Message = "Add successfuly"});
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<ProductController>/5
        [HttpPut("update/{id}")]
        public async Task<ActionResult> Update(int id, [FromForm] string data, List<IFormFile> images, [FromForm] string imageUrls)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<ProductRequest>(data);

                if (model == null)
                {
                    throw new Exception(message: "model error");
                }

                CheckModelValid(model);

                var url = JsonConvert.DeserializeObject<List<string>>(imageUrls);

                await _productService.Update(id, model, images, url ?? []);

                return Ok(new { Message = "Update successfuly" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _productService.Delete(id);
                return Ok(new { Message = "Delete successfuly" });
            }
            catch (Exception ex) 
            { 
                return BadRequest(ex.Message);
            }
        }

        private void CheckModelValid(ProductRequest model)
        {
            if (string.IsNullOrEmpty(model.Name)
                || string.IsNullOrEmpty(model.Brand)
                || string.IsNullOrEmpty(model.Packaging)
                || model.CategoryId == 0)
            {
                throw new Exception(message: "Require field is empty");
            }

            foreach (var detail in model.Details)
            {
                if (string.IsNullOrEmpty(detail.Content) || string.IsNullOrEmpty(detail.Content))
                {
                    throw new Exception(message: "Require fields is empty");
                }
            }
        }
    }
}
