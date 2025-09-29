using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoomRentalManagerServer.Domain.Interfaces;
using RoomRentalManagerServer.Domain.ModelEntities;
using RoomRentalManagerServer.Infrastructure.Data;

namespace RoomRentalManagerServer.Infrastructure.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly RoomRentalManagerServerDbContext _context;
        private readonly ILogger<PermissionRepository> _logger;
        public PermissionRepository(RoomRentalManagerServerDbContext context, ILogger<PermissionRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IQueryable<Permission>> GetAllQueryAsync()
        {
            try
            {
                return _context.Permission.AsQueryable().AsNoTracking();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all permissions: {ex.Message}");
                throw;
            }
        }
    }
}
