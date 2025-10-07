namespace RoomRentalManagerServer.Application.Interfaces
{
    public interface IRoleGroupPermissionAppService
    {
        Task<List<long>> GetActivePermissionByRoleGroupIdAsync(long roleGroupId);
        Task<bool> DeleteActivePermissionByRoleGroupIdAsync(long roleGroupId);
    }
}
