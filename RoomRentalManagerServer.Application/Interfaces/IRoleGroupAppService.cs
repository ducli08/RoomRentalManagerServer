using RoomRentalManagerServer.Application.Common.CommonDto;
using RoomRentalManagerServer.Application.Model.RoleGroupsModel.Dto;
using RoomRentalManagerServer.Domain.ModelEntities.RoleGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomRentalManagerServer.Application.Interfaces
{
    public interface IRoleGroupAppService
    {
        Task<PagedResultDto<RoleGroupDto>> GetAllRoleGroupsAsync(PagedRequestDto<RoleGroupFilterDto> pagedRequestRoleGroupDto);
        Task<RoleGroup> GetRoleGroupByIdAsync(long id);
        Task<bool> CreateOrEditRoleGroup(CreateOrEditRoleGroupDto input);
        Task<bool> UpdateAsync(RoleGroup roleGroup);
        Task<bool> AddAsync(RoleGroup roleGroup);
    }
}
