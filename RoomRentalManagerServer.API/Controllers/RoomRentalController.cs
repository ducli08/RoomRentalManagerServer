using Microsoft.AspNetCore.Mvc;
using RoomRentalManagerServer.Application.Interfaces;

namespace RoomRentalManagerServer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomRentalController : Controller
    {
        public readonly IUserAppService _userAppService;
        public readonly IRoomRentalAppService _roomRentalAppService;
        public RoomRentalController(IUserAppService userAppService, IRoomRentalAppService roomRentalAppService)
        {
            _userAppService = userAppService;
            _roomRentalAppService = roomRentalAppService;
        }
        [HttpGet("/getAllAsync")]
        public async Task<ActionResult> GetAllAsync()
        {
            var lstRoomRental = _roomRentalAppService.GetAllRoomRentalAsync();
            return Json(new { });
        }
    }
}
