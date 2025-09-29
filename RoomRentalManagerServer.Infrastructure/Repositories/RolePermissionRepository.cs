using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoomRentalManagerServer.Domain.Interfaces;
using RoomRentalManagerServer.Domain.ModelEntities;
using RoomRentalManagerServer.Infrastructure.Data;

namespace RoomRentalManagerServer.Infrastructure.Repositories
{
    public class RolePermissionRepository : IRolePermissionRepository
    {
        private readonly RoomRentalManagerServerDbContext _context;
        private readonly ILogger<RolePermissionRepository> _logger;
        public RolePermissionRepository(RoomRentalManagerServerDbContext context, ILogger<RolePermissionRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IQueryable<RolePermission>> GetAllQueryAsync()
        {
            try
            {
                return _context.RolePermission.AsQueryable().AsNoTracking();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all role permissions: {ex.Message}");
                throw;
            }
        }
    }
}
