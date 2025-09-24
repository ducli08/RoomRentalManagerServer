using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Domain.Interfaces.RoleGroupPermissionInterfaces;
using RoomRentalManagerServer.Domain.ModelEntities.RoleGroupPermission;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomRentalManagerServer.Application.Services
{
    public class RoleGroupPermissionAppService : IRoleGroupPermissionAppService
    {
        private readonly IRoleGroupPermissionRepository _repo;
        public RoleGroupPermissionAppService(IRoleGroupPermissionRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<long>> GetPermissionsByRoleGroupIdAsync(long roleGroupId)
        {
            var query = await _repo.GetByRoleGroupIdAsync(roleGroupId);
            var list = query.Select(x => x.RoleId).ToList();
            return list;
        }

        public async Task<bool> SetPermissionsForRoleGroupAsync(long roleGroupId, List<long> roleIds)
        {
            // remove existing and add new list
            var existingQuery = await _repo.GetByRoleGroupIdAsync(roleGroupId);
            var existing = existingQuery.ToList();

            // naive remove: delete existing entries
            foreach (var e in existing)
            {
                await _repo.DeleteAsync(e.Id);
            }

            foreach (var roleId in roleIds)
            {
                var entity = new RoleGroupPermission
                {
                    RoleGroupId = roleGroupId,
                    RoleId = roleId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system"
                };
                await _repo.AddAsync(entity);
            }

            return true;
        }
    }
}
