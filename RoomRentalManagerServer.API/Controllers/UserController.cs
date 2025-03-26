using Microsoft.AspNetCore.Mvc;
using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Application.Model.UsersModel.Dto;

namespace RoomRentalManagerServer.API.Controllers
{
    public class UserController : Controller
    {
        public readonly IUserAppService _userAppService;
        public UserController(IUserAppService userAppService)
        {
            _userAppService = userAppService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> CreateOrEditUser(CreateOrEditUserDto input)
        {
            var resCreateOrUpdate = await _userAppService.CreateOrEditUserAsync(input);
            var action = input.Id != null ? "edit" : "create";
            var res = new
            {
                code = resCreateOrUpdate ? 200 : 500,
                message = resCreateOrUpdate ? $"User {action} successfully" : $"Failed to {action} user"
            };
            return Json(res);
        }
    }
}
