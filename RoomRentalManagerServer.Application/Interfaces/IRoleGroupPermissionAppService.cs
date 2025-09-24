using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoomRentalManagerServer.Application.Interfaces
{
    public interface IRoleGroupPermissionAppService
    {
        Task<List<long>> GetPermissionsByRoleGroupIdAsync(long roleGroupId);
        Task<bool> SetPermissionsForRoleGroupAsync(long roleGroupId, List<long> roleIds);
    }
}
