using Microsoft.AspNetCore.Mvc;
using RoomRentalManagerServer.Application.Common.CommonDto;
using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Application.Model.RoleGroupsModel.Dto;
using RoomRentalManagerServer.Application.Model.RoomRentalsModel.Dto;
using RoomRentalManagerServer.Domain.ModelEntities.RoleGroups;

namespace RoomRentalManagerServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleGroupController : ControllerBase
    {
        private readonly IRoleGroupAppService _roleGroupAppService;
        private readonly ILogger<RoleGroupController> _logger;

        public RoleGroupController(IRoleGroupAppService roleGroupAppService, ILogger<RoleGroupController> logger)
        {
            _roleGroupAppService = roleGroupAppService;
            _logger = logger;
        }

        [HttpPost("getAllRoleGroupsAsync")]
        [ProducesResponseType(typeof(PagedResultDto<RoomRentalDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllRoleGroupsAsync([FromBody] PagedRequestDto<RoleGroupFilterDto> requestDto)
        {
            if (requestDto == null)
                return BadRequest(new { message = "Request body is required." });

            var roomRentals = await _roleGroupAppService.GetAllRoleGroupsAsync(requestDto);
            return Ok(roomRentals);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrEditRoleGroupDto input)
        {
            var res = await _roleGroupAppService.CreateOrEditRoleGroup(input);
            return Ok(new { success = res });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] CreateOrEditRoleGroupDto input)
        {
            if (input.Id == null || input.Id != id)
                return BadRequest(new { message = "Id mismatch" });

            var res = await _roleGroupAppService.CreateOrEditRoleGroup(input);
            return Ok(new { success = res });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _roleGroupAppService.UpdateAsync(new RoleGroup { Id = id }); // use update? Should call delete repo directly - but keep simple
            return NoContent();
        }
    }
}
