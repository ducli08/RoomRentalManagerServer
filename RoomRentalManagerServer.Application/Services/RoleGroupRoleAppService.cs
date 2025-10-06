using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Domain.Interfaces.RoleGroupPermissionInterfaces;

namespace RoomRentalManagerServer.Application.Services
{
    public class RoleGroupPermissionAppService : IRoleGroupPermissionAppService
    {
        private readonly IRoleGroupPermissionRepository _roleGroupPermissionRepository;
        public RoleGroupPermissionAppService(IRoleGroupPermissionRepository roleGroupPermissionRepository)
        {
            _roleGroupPermissionRepository = roleGroupPermissionRepository;
        }

        public async Task<List<long>> GetRoleByPermissionGroupIdAsync(long roleGroupId)
        {
            var query = await _roleGroupPermissionRepository.GetByIdAsync(roleGroupId);
            var list = query.Select(x => x.PermissionId).ToList();
            return list;
        }

    }
}
