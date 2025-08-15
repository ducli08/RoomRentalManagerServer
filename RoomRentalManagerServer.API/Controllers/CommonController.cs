using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RoomRentalManagerServer.Application.Common.CommonAppService;
using RoomRentalManagerServer.Application.Common.CommonDto;
using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Application.Model.RoomRentalsModel.Dto;

namespace RoomRentalManagerServer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommonController : Controller
    {
        public readonly ICommonAppService _commonAppService;
        public CommonController(ICommonAppService commonAppService)
        {
            _commonAppService = commonAppService;
        }
        [HttpPost("getSelectListItem")]
        public async Task<ActionResult<List<SelectListItem>>> GetSelectListItem(string type, string? cascadeValue)
        {
            var selectListItemDtos = await _commonAppService.GetCustomSelectListItem(type, cascadeValue);
            var selectListItems = selectListItemDtos.Select(item => new SelectListItem
            {
                Value = item.Value,
                Text = item.Text
            }).ToList();
            return Ok(selectListItems);
        }

        [HttpPost("getEnumSelectListItem")]
        public async Task<ActionResult<List<SelectListItemDto>>> GetEnumSelectListItem([FromQuery]string? enumType)
        {
            if (string.IsNullOrEmpty(enumType))
            {
                return BadRequest("Enum type is null!");
            }
            var selectListEnum = _commonAppService.GetEnumSelectListItem(enumType);
            return Ok(selectListEnum);
        }
    }
}
