﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaShop.Application.Models.Response;
using PharmaShop.Application.Services;
using PharmaShop.Infastructure.Models;
using System.Security.Claims;

namespace PharmaShop.Application.Controllers
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
        public async Task<ActionResult<CustomerInfoResponse>> UserInfo()
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                ApplicationUser user = await _userService.GetUserByUserNameAsync(username);

                if (user == null)
                {
                    throw new Exception(message: "Invalid user.");
                }

                CustomerInfoResponse response = new();

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
