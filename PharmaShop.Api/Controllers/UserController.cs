using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PharmaShop.Api.Models.Response;
using PharmaShop.Api.Services;
using PharmaShop.Infastructure.Models;
using System.Security.Claims;

namespace PharmaShop.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet("getuserinfo")]
        public async Task<ActionResult<CustomerInfoResponseModel>> UserInfo()
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                ApplicationUser user = await _userService.GetUserByUserNameAsync(username);

                if (user == null)
                {
                    throw new Exception(message: "Invalid user.");
                }

                CustomerInfoResponseModel response = new();

                response.FullName = user.FullName ?? "No name";
                response.Type = user.Type == null ? "No type" : user.Type.Name;
                response.Point = user.Point;

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
