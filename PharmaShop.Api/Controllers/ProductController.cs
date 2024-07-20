using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;
using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;
using PharmaShop.Application.Abtract;
using Newtonsoft.Json;
using PharmaShop.Api.Abtract;
using PharmaShop.Infastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace PharmaShop.Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductService _productService;

        public ProductController(IUnitOfWork unitOfWork, IProductService productService)
        {
            _unitOfWork = unitOfWork;
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

        [HttpPost("getpanigation")]
        public async Task<ActionResult<TableResponseModel<ProductResponseModel>>> GetPanigation([FromForm] string request)
        {
            var data = JsonConvert.DeserializeObject<TableRequestModel>(request);

            var (result, total) = await _unitOfWork.ProductRepository.GetProductPanigationAsync(data.PageIndex, data.PageSize, data.Keyword = string.Empty);

            List<ProductResponseModel> datas = [];

            var categories = await _unitOfWork.Table<Category>().ToListAsync();

            foreach (var item in result)
            {
                datas.Add(new ProductResponseModel
                {
                    Id = item.Id,
                    Name = item.Name ?? "",
                    Brand = item.Brand ?? "",
                    Packaging = item.Packaging ?? "",
                    Price = item.BigUnitPrice,
                    CategoryId = item.CategoryId,
                    CategoryName = categories.FirstOrDefault(c => c.Id == item.CategoryId).Name
                });
            }

            return Ok(new TableResponseModel<ProductResponseModel>
            {
                PageSize = data.PageSize,
                Datas = datas,
                Total = total
            });
        }

        [HttpPost("add")]  
        public async Task<ActionResult> Add([FromForm] string data, List<IFormFile> images)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<ProductRequestModel>(data);

                if (model == null)
                {
                    throw new Exception(message: "model error");
                }

                if (string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Brand) || string.IsNullOrEmpty(model.Packaging)
                || model.CategoryId == 0)
                {
                    throw new Exception(message: "Require field is empty");
                }

                await _productService.Add(model, images);

                return Ok(new {Message = "Add successfuly"});
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
