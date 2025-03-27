using Microsoft.AspNetCore.Mvc;
using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Application.Model.UsersModel.Dto;

namespace RoomRentalManagerServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly IUserAppService _userAppService;
        public UserController(IUserAppService userAppService)
        {
            _userAppService = userAppService;
        }
        [HttpPost("createOrEditUser")]
        public async Task<ActionResult> CreateOrEditUser(CreateOrEditUserDto input)
        {
            var resCreateOrUpdate = await _userAppService.CreateOrEditUserAsync(input);
            var action = input.Id != null ? "edit" : "create";
            var res = new
            {
                code = resCreateOrUpdate ? 200 : 500,
                message = resCreateOrUpdate ? $"User {action} successfully" : $"Failed to {action} user"
            };
            return Ok(res);
        }
        [HttpGet("editingPopupRead")]
        public async Task<List<UserDto>> EditingPopupRead()
        {
            var lstUser = await _userAppService.GetAllUsersAsync();
            return lstUser;
        }
    }
}
