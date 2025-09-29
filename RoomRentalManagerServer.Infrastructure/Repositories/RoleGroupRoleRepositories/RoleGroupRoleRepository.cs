using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoomRentalManagerServer.Domain.Interfaces.RoleGroupRoleInterfaces;
using RoomRentalManagerServer.Domain.ModelEntities.RoleGroupRole;
using RoomRentalManagerServer.Infrastructure.Data;

namespace RoomRentalManagerServer.Infrastructure.Repositories.RoleGroupRoleRepositories
{
    public class RoleGroupRoleRepository : IRoleGroupRoleRepository
    {
        private readonly RoomRentalManagerServerDbContext _context;
        private readonly ILogger<RoleGroupRoleRepository> _logger;
        public RoleGroupRoleRepository(RoomRentalManagerServerDbContext context, ILogger<RoleGroupRoleRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<RoleGroupRole> AddAsync(RoleGroupRole entity)
        {
            try
            {
                await _context.RoleGroupRole.AddAsync(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to add RoleGroupRole: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            try
            {
                var entity = await _context.RoleGroupRole.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null) return false;
                _context.RoleGroupRole.Remove(entity);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to delete RoleGroupRole: {ex.Message}");
                throw;
            }
        }

        public async Task<IQueryable<RoleGroupRole>> GetAllQueryAsync()
        {
            try
            {
                return _context.RoleGroupRole.AsQueryable().AsNoTracking();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all RoleGroupRole: {ex.Message}");
                throw;
            }
        }

        public async Task<IQueryable<RoleGroupRole>> GetByIdAsync(long id)
        {
            try
            {
                return _context.RoleGroupRole.AsNoTracking().Where(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get RoleGroupRole by id: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(RoleGroupRole entity)
        {
            try
            {
                var data = _context.RoleGroupRole.FirstOrDefault(x => x.Id == entity.Id);
                if (data == null) return false;
                _context.RoleGroupRole.Update(entity);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update RoleGroupRole: {ex.Message}");
                throw;
            }
        }

        public async Task<IQueryable<RoleGroupRole>> GetByRoleGroupIdAsync(long roleGroupId)
        {
            try
            {
                return _context.RoleGroupRole.Where(x => x.RoleGroupId == roleGroupId).AsQueryable().AsNoTracking();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get RoleGroupRole by roleGroupId: {ex.Message}");
                throw;
            }
        }
    }
}
