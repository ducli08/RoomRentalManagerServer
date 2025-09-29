using RoomRentalManagerServer.Domain.ModelEntities.RoleGroupRole;

namespace RoomRentalManagerServer.Domain.Interfaces.RoleGroupRoleInterfaces
{
    public interface IRoleGroupRoleRepository
    {
        Task<RoleGroupRole> AddAsync(RoleGroupRole entity);
        Task<bool> DeleteAsync(long id);
        Task<IQueryable<RoleGroupRole>> GetByIdAsync(long id);
        Task<bool> UpdateAsync(RoleGroupRole entity);
        Task<IQueryable<RoleGroupRole>> GetAllQueryAsync();
        Task<IQueryable<RoleGroupRole>> GetByRoleGroupIdAsync(long roleGroupId);
    }
}
