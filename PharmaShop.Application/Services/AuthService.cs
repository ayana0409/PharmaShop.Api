using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PharmaShop.Application.Abtract;
using PharmaShop.Application.Models.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using PharmaShop.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using PharmaShop.Domain.Abtract;
using Microsoft.EntityFrameworkCore;

namespace PharmaShop.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<AuthResponse> AuthenticateAsync(Models.Request.LoginRequest user)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                throw new ApplicationException(message: "Username or password must be not empty.");
            }

            var loginUser = await _userManager.FindByNameAsync(user.Username);
            if (loginUser == null)
            {
                throw new ApplicationException(message: "Invalid username or password.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(loginUser, user.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                throw new ApplicationException(message: "Invalid username or password.");
            }

            return new AuthResponse
            {
                Token = await GenerateJwtToken(loginUser)
            };
        }

        public async Task<AuthResponse> ExternalLoginAsync(string email, string name)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ApplicationException("Email must not be empty.");
            }

            var loginUser = await _userManager.FindByNameAsync(email);
            if (loginUser == null)
            {
                int firstTypeId = _unitOfWork.Table<UserType>()
                                                .OrderBy(t => t.Point)
                                                .FirstOrDefaultAsync().Id;
                var newUser = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = name,
                    TypeId = firstTypeId,
                    IsActive = true,
                };

                var createResult = await _userManager.CreateAsync(newUser);
                if (!createResult.Succeeded)
                {
                    throw new ApplicationException("User creation failed.");
                }

                loginUser = newUser;
            }

            var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(loginUser);
            await _signInManager.Context.SignInAsync(IdentityConstants.ApplicationScheme, claimsPrincipal);

            return new AuthResponse
            {
                Token = await GenerateJwtToken(loginUser)
            };
        }



        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                _configuration["Jwt:ValidIssuer"],
                _configuration["Jwt:ValidAudience"],
                
                claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
