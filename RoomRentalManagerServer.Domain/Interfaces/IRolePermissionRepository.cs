using RoomRentalManagerServer.Domain.ModelEntities;

namespace RoomRentalManagerServer.Domain.Interfaces
{
    public interface IRolePermissionRepository
    {
        Task<IQueryable<RolePermission>> GetAllQueryAsync();
    }
}
