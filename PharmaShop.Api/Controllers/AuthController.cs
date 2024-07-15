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
        public async Task<ActionResult<AuthResponseModel>> Login(LoginRequestModel loginUser)
        {
            try
            {
                return await _authService.AuthenticateAsync(loginUser);
            }
            catch (ApplicationException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
