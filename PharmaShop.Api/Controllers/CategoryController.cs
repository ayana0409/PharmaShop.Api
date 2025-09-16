using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;
using PharmaShop.Application.Abtract;

namespace PharmaShop.Application.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("table")]
        public async Task<ActionResult<IEnumerable<CategoryTableResponse>>> GetForTable()
        {
            try
            {
                var result = await _categoryService.GetForTableAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Add([FromBody] CategoryRequest value)
        {
            if (string.IsNullOrEmpty(value.Name))
            {
                throw new Exception(message: "Category name must be not empty.");
            }
            try
            {
                await _categoryService.AddAsync(value);
                return Ok(new { Message = "Add successfuly" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id,[FromBody] CategoryRequest value)
        {
            try
            {
                if (string.IsNullOrEmpty(value.Name))
                {
                    throw new Exception(message: "Category name must be not empty.");
                }

                await _categoryService.UpdateAsync(id, value);

                return Ok(new { Message = "Update successfuly" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _categoryService.DeleteAsync(id);
                
                return Ok(new { Message = "Delete successfuly" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
