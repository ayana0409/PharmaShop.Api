using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using PharmaShop.Application.Abtract;
using PharmaShop.Application.Models.Request;
using PharmaShop.Application.Models.Response;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using PharmaShop.Api.DTOs;
using System.Text;

namespace PharmaShop.Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpGet("google-client-id")]
        public IActionResult GetGoogleClientId()
        {
            var clientId = _configuration["GoogleAuthentication:ClientID"];
            return Ok(new { ClientId = clientId });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest loginUser)
        {
            try
            {
                return await _authService.AuthenticateAsync(loginUser);
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            var payload = await VerifyGoogleTokenAsync(request.TokenId);
            if (payload == null)
            {
                return NotFound("Invalid Google token.");
            }

            var email = payload.Email;
            var name = payload.Name;

            var authResponse = await _authService.ExternalLoginAsync(email, name);
            if (authResponse != null)
            {
                return Ok(authResponse);
            }

            return BadRequest("Login failed.");
        }

        private async Task<GooglePayload> VerifyGoogleTokenAsync(string tokenId)
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"https://oauth2.googleapis.com/tokeninfo?id_token={tokenId}");

            if (!response.IsSuccessStatusCode)
            {
                // Xử lý lỗi
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var payload = Newtonsoft.Json.JsonConvert.DeserializeObject<GooglePayload>(content);

            return payload;
        }
    }
}
