using Microsoft.AspNetCore.Mvc;
using RoomRentalManagerServer.Application.Common.CommonDto;
using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Application.Model.RoomRentalsModel.Dto;
using RoomRentalManagerServer.Application.Model.UsersModel.Dto;

namespace RoomRentalManagerServer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomRentalController : Controller
    {
        public readonly IRoomRentalAppService _roomRentalAppService;
        public RoomRentalController(IUserAppService userAppService, IRoomRentalAppService roomRentalAppService)
        {
            _roomRentalAppService = roomRentalAppService;
        }
        [HttpPost("getAllRoomRentalAsync")]
        [ProducesResponseType(typeof(PagedResultDto<RoomRentalDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllRoomRentalAsync([FromBody] PagedRequestDto<RoomRentalFilterDto> requestDto)
        {
            var roomRentals = await _roomRentalAppService.GetAllRoomRentalAsync(requestDto);
            return Ok(roomRentals);
        }
        [HttpPost("createOrEditRoomRental")]
        public async Task<ActionResult> CreateOrEditRoomRental(CreateOrEditRoomRentalDto input)
        {
            var resCreateOrUpdate = await _roomRentalAppService.CreateOrEditRoomRentalAsync(input);
            var action = input.Id != null ? "edit" : "create";
            var res = new
            {
                code = resCreateOrUpdate ? 200 : 500,
                message = resCreateOrUpdate ? $"User {action} successfully" : $"Failed to {action} user"
            };
            return Ok(res);
        }
    }
}
