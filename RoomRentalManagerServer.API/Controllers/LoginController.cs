using Microsoft.AspNetCore.Mvc;
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
        private readonly IConfiguration _configuration;
        public LoginController(IUserAppService userAppService, IRedisCacheService redisCacheService, ILogger<UserController> logger,
            IRoleGroupAppService roleGroupAppService, IJwtTokenAppService jwtTokenAppService, IConfiguration configuration)
        {
            _userAppService = userAppService;
            _redisCacheService = redisCacheService;
            _roleGroupAppService = roleGroupAppService;
            _logger = logger;
            _jwtTokenAppService = jwtTokenAppService;
            _configuration = configuration;
        }
        [HttpPost]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login(string username, string password, bool rememberMe)
        {
            var expiresInMinutes = _configuration.GetSection("Jwt")["ExpiresInMinutes"];
            var expires = DateTime.UtcNow.AddMinutes(int.Parse(expiresInMinutes ?? "30"));
            var user = await _userAppService.Authentication(username, password);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var accessToken = _jwtTokenAppService.GenerateToken(user.Id, user.Name, expires);

            string? refreshToken = null;
            DateTime? refreshExpiresAt = null;
            int? refreshExpiresIn = null;
            if (rememberMe)
            {
                refreshToken = _jwtTokenAppService.GenerateRefreshToken();
                var expiry = TimeSpan.FromDays(7);
                await _redisCacheService.SetAsync<string>($"refresh_{user.Id}", new List<string> { refreshToken }, expiry);
                refreshExpiresAt = DateTime.UtcNow.Add(expiry);
                refreshExpiresIn = (int)expiry.TotalSeconds;
            }

            // Optionally cache user info
            var userCacheExpiry = rememberMe ? TimeSpan.FromDays(7) : TimeSpan.FromHours(1);
            await _redisCache_service_SetUserAsync(user.Id.ToString(), user, userCacheExpiry);

            var loginResponseDto = new LoginResponseDto
            {
               Message = "Login successful",
               AccessToken = accessToken,
               RefreshToken = refreshToken,
               User = user,
               ExpiresAt = expires,
               ExpiresIn = (int)(expires - DateTime.UtcNow).TotalSeconds,
               RefreshExpiresAt = refreshExpiresAt,
               RefreshExpiresIn = refreshExpiresIn
            };

            // if refresh provided, include optionally as additional headers? For now attach to response body via dynamic
            return Ok(loginResponseDto);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.RefreshToken))
                return BadRequest(new { message = "Invalid refresh request" });

            var key = $"refresh_{request.UserId}";
            var stored = await _redisCacheService.GetAsync<string>(key);
            if (stored == null || stored.Count == 0)
                return Unauthorized(new { message = "Refresh token not found" });

            if (!stored.Contains(request.RefreshToken))
                return Unauthorized(new { message = "Invalid refresh token" });

            // generate new access token (short lived)
            var newAccessExpiry = DateTime.UtcNow.AddHours(1);
            // get username from cached user if available
            var cachedUserList = await _redisCacheService.GetAsync<UserDto>(request.UserId.ToString());
            var cachedUser = cachedUserList?.FirstOrDefault();
            var userName = cachedUser?.Name ?? string.Empty;
            var newAccessToken = _jwtTokenAppService.GenerateToken(request.UserId, userName, newAccessExpiry);

            string? newRefreshToken = null;
            DateTime? newRefreshExpiresAt = null;
            int? newRefreshExpiresIn = null;
            if (request.RememberMe)
            {
                // rotate refresh token
                newRefreshToken = _jwtTokenAppService.GenerateRefreshToken();
                var expiry = TimeSpan.FromDays(7);
                await _redisCacheService.SetAsync<string>(key, new List<string> { newRefreshToken }, expiry);
                newRefreshExpiresAt = DateTime.UtcNow.Add(expiry);
                newRefreshExpiresIn = (int)expiry.TotalSeconds;
            }

            var response = new RefreshResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
            return Ok(response);
        }

        // wrapper helper to set cached user as list<UserDto> to match existing Redis API
        private async Task _redisCache_service_SetUserAsync(string key, UserDto user, TimeSpan? expiry)
        {
            await _redisCacheService.SetAsync(key, new List<UserDto> { user }, expiry);
        }
    }
}
