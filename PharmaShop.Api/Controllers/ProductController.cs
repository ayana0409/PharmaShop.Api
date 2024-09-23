using Microsoft.AspNetCore.Mvc;
using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;
using Newtonsoft.Json;
using PharmaShop.Application.Abtract;
using Microsoft.AspNetCore.Authorization;
using PharmaShop.Application.Models.Response.Product;

namespace PharmaShop.Application.Controllers
{
    /// <summary>
    /// Quản lý sản phẩm.
    /// </summary>
    [Authorize(Roles = "SuperAdmin")]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        /// <summary>
        /// Lấy thông tin sản phẩm theo Id.
        /// </summary>
        /// <param name="id">Id của sản phẩm.</param>
        /// <returns>Sản phẩm tương ứng với Id.</returns>
        /// <response code="200">Trả về sản phẩm</response>
        /// <response code="404">Không tìm thấy sản phẩm</response>
        /// <response code="400">Lỗi tham số đầu vào</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponse>> Get(int id)
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

        /// <summary>
        /// Lấy thông tin sản phẩm theo trang.
        /// </summary>
        /// <param name="pageIndex">Số thứ tự trang</param>
        /// <param name="pageSize">Số phần tử cần lấy</param>
        /// <param name="keyword">Tìm theo tên, mã chứa keyword</param>
        /// <returns>Danh sách sản phẩm tương ứng.</returns>
        /// <response code="200">Trả về sản phẩm</response>
        /// <response code="404">Không tìm thấy sản phẩm</response>
        [HttpGet]
        public async Task<ActionResult<TableResponse<ProductSummary>>> GetPagigation([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 5, [FromQuery] string keyword = "")
        {

            try
            {
                var result = await _productService.GetPagigation(new TableRequest
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

        /// <summary>
        /// Thêm một sản phẩm mới và upload hình ảnh sản phẩm.
        /// </summary>
        /// <param name="request">Đối tượng yêu cầu chứa thông tin sản phẩm.</param>
        /// <param name="images">Danh sách các file hình ảnh cần upload cho sản phẩm.</param>
        /// <returns>Kết quả của hành động thêm sản phẩm.</returns>
        /// <response code="200">Thêm sản phẩm thành công, trả về thông báo xác nhận.</response>
        /// <response code="400">Dữ liệu đầu vào không hợp lệ hoặc xảy ra lỗi khi xử lý yêu cầu.</response>
        [HttpPost]
        public async Task<ActionResult> Add([FromForm] string request, List<IFormFile> images)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<ProductRequest>(request) ?? throw new Exception(message: "model error");

                CheckModelValid(data);

                await _productService.Add(data, images);

                return Ok(new {Message = "Add successfuly"});
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Cập nhật thông tin sản phẩm dựa trên ID.
        /// </summary>
        /// <param name="id">ID của sản phẩm cần cập nhật.</param>
        /// <param name="request">Chuỗi JSON chứa thông tin cập nhật của sản phẩm (ProductRequest).</param>
        /// <param name="images">Danh sách các tệp hình ảnh mới được tải lên để cập nhật sản phẩm.</param>
        /// <param name="imageUrls">Chuỗi JSON chứa danh sách URL của các hình ảnh hiện có.</param>
        /// <returns>Kết quả của hành động cập nhật sản phẩm.</returns>
        /// <response code="200">Cập nhật sản phẩm thành công, trả về thông báo xác nhận.</response>
        /// <response code="400">Dữ liệu đầu vào không hợp lệ hoặc có lỗi trong quá trình cập nhật, trả về thông báo lỗi.</response>
        /// <response code="404">Không tìm thấy sản phẩm với ID tương ứng.</response>
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromForm] string request, List<IFormFile> images, [FromForm] string imageUrls)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<ProductRequest>(request) ?? throw new Exception(message: "model error");

                CheckModelValid(data);

                var url = JsonConvert.DeserializeObject<List<string>>(imageUrls);

                await _productService.Update(id, data, images, url ?? []);

                return Ok(new { Message = "Update successfuly" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Xóa sản phẩm dựa trên ID.
        /// </summary>
        /// <param name="id">ID của sản phẩm cần xóa.</param>
        /// <returns>Kết quả của hành động xóa sản phẩm.</returns>
        /// <response code="200">Xóa sản phẩm thành công, trả về thông báo xác nhận.</response>
        /// <response code="400">Có lỗi xảy ra trong quá trình xóa sản phẩm, trả về thông báo lỗi.</response>
        /// <response code="404">Sản phẩm không tồn tại, trả về thông báo không tìm thấy.</response>
        [HttpDelete("{id}")]
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

        private static void CheckModelValid(ProductRequest model)
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
