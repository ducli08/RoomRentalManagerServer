using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RoomRentalManagerServer.Application.Common;
using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Application.Model.UsersModel.Dto;
using RoomRentalManagerServer.Domain.Interfaces.RedisCache;

namespace RoomRentalManagerServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly IUserAppService _userAppService;
        public readonly IProvinceAppService _provinceAppService;
        public readonly IDistrictAppService _districtAppService;
        public readonly IWardAppService _wardAppService;
        public readonly IRedisCacheService _redisCacheService;
        public UserController(IUserAppService userAppService, IProvinceAppService provinceAppService, IDistrictAppService districtAppService,
            IWardAppService wardAppService, IRedisCacheService redisCacheService)
        {
            _userAppService = userAppService;
            _provinceAppService = provinceAppService;
            _districtAppService = districtAppService;
            _wardAppService = wardAppService;
            _redisCacheService = redisCacheService;
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
        [HttpPost("editingPopupRead")]
        [ProducesResponseType(typeof(PagedResultDto<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> EditingPopupRead([FromBody] PagedRequestDto requestDto)
        {
            var result = await _userAppService.GetAllUsersAsync(requestDto);
            return Ok(result);
        }

        [HttpPost("getAllProvince")]
        public async Task<List<SelectListItem>> GetAllProvinceAsync()
        {
            var provinceQuery = await _provinceAppService.GetAllProvincesAsync();
            var selectList = provinceQuery.Select(p => new SelectListItem
            {
                Value = p.Code,
                Text = p.Name
            }).ToList();
            return selectList;
        }

        [HttpPost("getAllDistrict")]
        public async Task<List<SelectListItem>> GetAllDistrictAsync(string? provinceCode)
        {
            var districtQuery = await _districtAppService.GetAllDistrictsAsync(provinceCode);
            var selectList = districtQuery.Select(d => new SelectListItem
            {
                Value = d.Code,
                Text = d.Name
            }).ToList();
            return selectList;
        }

        [HttpPost("getAllWard")]
        public async Task<List<SelectListItem>> GetAllWardAsync(string? districtCode)
        {
            var wardQuery = await _wardAppService.GetAllWardsAsync(districtCode);
            var selectList = wardQuery.Select(w => new SelectListItem
            {
                Value = w.Code,
                Text = w.Name
            }).ToList();
            return selectList;
        }
    }
}
