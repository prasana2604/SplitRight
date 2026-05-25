using Microsoft.AspNetCore.Mvc;
using SplitRight.API.Services;
using SplitRight.API.Models.Entities;
using SplitRight.API.Models;
using SplitRightApi.cs.Models;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using System.Security.Claims;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Authorization;
namespace SplitRightApi.cs.Controller
{

    [ApiController]
    [Route("api/[Controller]")]
    public class AuthController : ControllerBase

    {
        private readonly IAuthService _AuthService;

        public AuthController (IAuthService authService)
        {
            _AuthService = authService;
        }


        [Authorize]

        [HttpGet("me")]

        public IActionResult GetMe()
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var Email = User.FindFirst(ClaimTypes.Email)?.Value;

            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new { UserId, Email, role });

        }

      

        [HttpPost("Register")]

        public async Task<IActionResult>Register(RegisterDto dto)
        {
            var result = await _AuthService.RegisterAsync(dto);

            if(result == "Email already registered")
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("Login")]

        public async Task<IActionResult>Login(LoginDto dto)
        {
            var result = await _AuthService.LoginAsync(dto);
            if (result == "Invalid Email or Password")
            {
                return Unauthorized(result);
            }
            return Ok(new { token = result });
        }
    }
}
