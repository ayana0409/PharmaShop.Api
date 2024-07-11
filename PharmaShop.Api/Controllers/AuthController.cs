using Microsoft.AspNetCore.Mvc;
using PharmaShop.Api.Abtract;
using PharmaShop.Api.Models.Request;
using PharmaShop.Api.Models.Response;

namespace PharmaShop.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseModel>> Login(LoginRequestModel model)
        {
            try
            {
                var token = await _authService.AuthenticateAsync(model.Username, model.Password);
                return new AuthResponseModel { Token = token };
            }
            catch (ApplicationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
