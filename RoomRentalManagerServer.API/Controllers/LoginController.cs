using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RoomRentalManagerServer.Application.Common;
using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Application.Model.Login.Dto;
using RoomRentalManagerServer.Application.Model.UsersModel.Dto;
using RoomRentalManagerServer.Domain.Interfaces.RedisCache;

namespace RoomRentalManagerServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserAppService _userAppService;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IRoleGroupAppService _roleGroupAppService;
        private readonly ILogger<UserController> _logger;
        private readonly IJwtTokenAppService _jwtTokenAppService;
        public LoginController(IUserAppService userAppService, IRedisCacheService redisCacheService, ILogger<UserController> logger,
            IRoleGroupAppService roleGroupAppService, IJwtTokenAppService jwtTokenAppService)
        {
            _userAppService = userAppService;
            _redisCacheService = redisCacheService;
            _roleGroupAppService = roleGroupAppService;
            _logger = logger;
            _jwtTokenAppService = jwtTokenAppService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login(string username, string password, bool rememberMe)
        {
            var expires = rememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddHours(1);
            var user = await _userAppService.Authentication(username, password);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }
            var token = _jwtTokenAppService.GenerateToken(user.Id, expires);

            // Optionally cache user info
            var expiry = rememberMe ? TimeSpan.FromDays(7) : TimeSpan.FromHours(1);
            await _redisCacheService.SetAsync(user.Id.ToString(), new List<UserDto> { user }, expiry);
            var loginResponseDto = new LoginResponseDto
            {
               Message = "Login successful",
               AccessToken = token,
               User = user
            };
            return Ok(loginResponseDto);
        }
    }
}
