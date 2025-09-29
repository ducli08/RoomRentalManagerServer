using RoomRentalManagerServer.Domain.ModelEntities;

namespace RoomRentalManagerServer.Domain.Interfaces
{
    public interface IPermissionRepository
    {
        Task<IQueryable<Permission>> GetAllQueryAsync();
    }
}
