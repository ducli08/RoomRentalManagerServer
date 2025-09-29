using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Domain.Interfaces.RoleGroupRoleInterfaces;
using RoomRentalManagerServer.Domain.ModelEntities.RoleGroupRole;

namespace RoomRentalManagerServer.Application.Services
{
    public class RoleGroupRoleAppService : IRoleGroupRoleAppService
    {
        private readonly IRoleGroupRoleRepository _roleGroupRoleRepository;
        public RoleGroupRoleAppService(IRoleGroupRoleRepository roleGroupRoleRepository)
        {
            _roleGroupRoleRepository = roleGroupRoleRepository;
        }

        public async Task<List<long>> GetRoleByRoleGroupIdAsync(long roleGroupId)
        {
            var query = await _roleGroupRoleRepository.GetByIdAsync(roleGroupId);
            var list = query.Select(x => x.RoleId).ToList();
            return list;
        }

    }
}
