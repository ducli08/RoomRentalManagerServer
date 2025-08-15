using Microsoft.AspNetCore.Mvc;
using RoomRentalManagerServer.Application.Common.CommonDto;
using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Application.Model.RoomRentalsModel.Dto;

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
        [HttpPost("uploadImageDescription")]
        public async Task<IActionResult> UploadImageDescription([FromForm] List<IFormFile> uploadImages)
        {
            if (uploadImages == null || !uploadImages.Any())
                return BadRequest(new { message = "No files received." });

            var savedPaths = new List<string>();
            var uploadsRootFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "room-images");

            if (!Directory.Exists(uploadsRootFolder))
                Directory.CreateDirectory(uploadsRootFolder);

            foreach (var file in uploadImages)
            {
                if (file.Length > 0)
                {
                    var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(uploadsRootFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Return relative path for client use
                    var relativePath = $"/uploads/room-images/{uniqueFileName}";
                    savedPaths.Add(relativePath);
                }
            }

            return Ok(new { paths = savedPaths });
        }
    }
}
