using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PharmaShop.Api.Abtract;
using PharmaShop.Api.Services;
using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;
using PharmaShop.Application.Services;
using PharmaShop.Infastructure.Models;
using System.Security.Claims;

namespace PharmaShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IImportService _importService;

        public ImportController(IUserService userService, IImportService importService)
        {
            _userService = userService;
            _importService = importService;
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<ImportResponse>> Get(int id)
        {
            try
            {
                return await _importService.GetAsync(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("getpanigation")]
        public async Task<ActionResult<TableResponse<ProductResponse>>> GetPanigation([FromForm] string request)
        {
            var data = JsonConvert.DeserializeObject<TableRequest>(request);

            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (username == null) { return Unauthorized(); }

            ApplicationUser? user = await _userService.GetUserByUserNameAsync(username);

            if (user == null)
            {
                return Unauthorized();
            }

            if (data == null)
            {
                return BadRequest();
            }

            return Ok(await _importService.GetPanigationAsync(user.Id, data));
        }

        [HttpGet("getdetails/{id}")]
        public async Task<ActionResult<List<ImportDetailResponse>>> GetDetails(int id)
        {
            try
            {
                return await _importService.GetDetailsByIdAsync(id);
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

        [HttpPost("create")]
        public async Task<ActionResult<int>> Create()
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (username == null) { return Unauthorized(); }

                ApplicationUser? user = await _userService.GetUserByUserNameAsync(username);

                if (user == null) 
                {
                    return Unauthorized();
                }

                var result = await _importService.CreateImportAsync(user.Id);

                return result;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<ImportController>/5
        [HttpPut("Complete/{id}")]
        public async Task<ActionResult> Complete(int id)
        {
            try
            {
                await _importService.CompleteImportAsync(id);
                return Ok();
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
                await _importService.DeleteImportAsync(id);
                return Ok(new { Message = "Delete successfuly" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("adddetail")]
        public async Task<ActionResult> AddDetail([FromForm] string request)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<ImportDetailRequest>(request);

                if (data == null)
                {
                    return BadRequest();
                }

                await _importService.AddDetailsAsync(data);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("updatedetail")]
        public async Task<ActionResult> UpdateDetail([FromForm] string data)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<ImportDetailRequest>(data);

                if (model == null)
                {
                    throw new Exception(message: "model error");
                }

                CheckModelValid(model);

                await _importService.UpdateDetailAsync(model);

                return Ok(new { Message = "Update successfuly" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("removedetail")]
        public async Task<ActionResult> RemoveDetail([FromForm] int importId, [FromForm] int productId)
        {
            try
            {
                await _importService.RemoveDetails(importId, productId);

                return Ok(new { Message = "Remove successfuly" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private void CheckModelValid(ImportDetailRequest model)
        {
            if (string.IsNullOrEmpty(model.BatchNumber)
                || string.IsNullOrEmpty(model.ManufactureDate)
                || model.Expiry <=0
                || model.Cost<=0)
            {
                throw new Exception(message: "Require field is empty");
            }
        }
    }
}
