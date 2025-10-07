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

        public async Task<List<long>> GetActivePermissionByRoleGroupIdAsync(long roleGroupId)
        {
            var query = await _roleGroupPermissionRepository.GetByRoleGroupIdAsync(roleGroupId);
            var list = query.Select(x => x.PermissionId).ToList();
            return list;
        }

        public async Task<bool> DeleteActivePermissionByRoleGroupIdAsync(long roleGroupId)
        {
            var res = await _roleGroupPermissionRepository.DeleteActivePermissionByRoleGroupIdAsync(roleGroupId);
            return res;
        }

    }
}
