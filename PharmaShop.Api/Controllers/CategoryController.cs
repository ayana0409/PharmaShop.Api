using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;
using PharmaShop.Application.Abtract;
using PharmaShop.Infastructure.Entities;

namespace PharmaShop.Application.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("categorytable")]
        public List<CategoryTableResponse> GetForTable()
        {
            var data = _unitOfWork.Table<Category>().Where(c => c.IsAcvive == true).ToList();

            List<CategoryTableResponse> result = [];

            foreach (var item in data)
            {
                result.Add(new CategoryTableResponse
                {
                    Id = item.Id,
                    Name = item.Name ?? "Noname",
                    ParentId = item.ParentCategory == null ? 0 : item.ParentId,
                    ParentName = item.ParentCategory == null ? "" : item.ParentCategory.Name
                });
            }

            return result;
        }

        // POST api/<CategoryController>
        [HttpPost("addcategory")]
        public async Task<ActionResult> AddCategory([FromBody] CategoryRequest value)
        {
            try
            {
                if (string.IsNullOrEmpty(value.Name)) 
                {
                    throw new Exception(message: "Category name must be not empty.");
                }

                await _unitOfWork.Table<Category>().AddAsync(new Category
                {
                    Name = value.Name,
                    ParentId = value.ParentId != 0 ? value.ParentId : null,
                });

                await _unitOfWork.SaveChangeAsync();
                return Ok(new { Message = "Add successfuly" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<CategoryController>/5
        [HttpPut("update")]
        public ActionResult Update([FromBody] CategoryRequest value)
        {
            try
            {
                if (string.IsNullOrEmpty(value.Name))
                {
                    throw new Exception(message: "Category name must be not empty.");
                }

                Category? category = _unitOfWork.Table<Category>().FirstOrDefault(c => c.Id == value.Id);

                if (category == null)
                {
                    throw new Exception(message: "Invalid category");
                }

                category.Name = value.Name;
                category.ParentId = value.ParentId == 0 ? null : value.ParentId;

                _unitOfWork.SaveChangeAsync();

                return Ok(new { Message = "Update successfuly" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                Category? category = _unitOfWork.Table<Category>().FirstOrDefault(c => c.Id == id);

                if (category == null)
                {
                    throw new Exception(message: "Invalid category");
                }
                category.IsAcvive = false;

                _unitOfWork.SaveChangeAsync();
                
                return Ok(new { Message = "Delete successfuly" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
