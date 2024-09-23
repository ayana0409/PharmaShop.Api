using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaShop.Application.Abtract;
using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Request.Account;
using PharmaShop.Application.Models.Response.Accounts;
using PharmaShop.Application.Models.Response.Order;

namespace PharmaShop.Api.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpGet]
        public async  Task<List<AccountResponse>> Get()
        {
            var result = await _accountService.GetAllAsync();

            return result;
        }

        [HttpGet("panigation")]
        public async Task<ActionResult<List<OrderResponse>>> GetPagination([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 5, [FromQuery] bool requireRole = false, [FromQuery] string? keyword = null)
        {
            try
            {
                var result = await _accountService.GetPanigationAsync(pageIndex, pageSize, requireRole, keyword);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AccountResponse>> Get(string id)
        {
            try
            {
                var result = await _accountService.GetByIdAsync(id);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("roles")]
        public async Task<List<RoleResponse>> GetRoles()
        {
            var result = await _accountService.GetRolesAsync();

            return result;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AccountRequest request)
        {
            try
            {
                await _accountService.CreateAsync(request);

                return Ok();
            }
            catch (InvalidOperationException ex) 
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(string id, [FromBody] AccountRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            try
            {
                await _accountService.UpdateAsync(request);

                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/<AccountsController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                await _accountService.RemoveAsync(id);

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
