using RoomRentalManagerServer.Domain.ModelEntities.RoleGroupPermission;

namespace RoomRentalManagerServer.Domain.Interfaces.RoleGroupPermissionInterfaces
{
    public interface IRoleGroupPermissionRepository
    {
        Task<RoleGroupRole> AddAsync(RoleGroupRole entity);
        Task<bool> DeleteAsync(long id);
        Task<RoleGroupRole> GetByIdAsync(long id);
        Task<bool> UpdateAsync(RoleGroupRole entity);
        Task<IQueryable<RoleGroupRole>> GetAllQueryAsync();
        Task<IQueryable<RoleGroupRole>> GetByRoleGroupIdAsync(long roleGroupId);
    }
}
