using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoomRentalManagerServer.Domain.Interfaces.RoleGroupInterfaces;
using RoomRentalManagerServer.Domain.Interfaces.RoleInterfaces;
using RoomRentalManagerServer.Domain.ModelEntities.RoleGroups;
using RoomRentalManagerServer.Domain.ModelEntities.RoomRentals;
using RoomRentalManagerServer.Infrastructure.Data;
using RoomRentalManagerServer.Infrastructure.Repositories.RoleRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomRentalManagerServer.Infrastructure.Repositories.RoleGroupRepositories
{
    public class RoleGroupRepository : IRoleGroupRepository
    {
        private readonly RoomRentalManagerServerDbContext _context;
        private readonly ILogger<RoleGroupRepository> _logger;
        public RoleGroupRepository(RoomRentalManagerServerDbContext context, ILogger<RoleGroupRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<RoleGroup> AddAsync(RoleGroup roleGroup)
        {
            try
            {
                await _context.RoleGroup.AddAsync(roleGroup);
                await _context.SaveChangesAsync();
                return roleGroup;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to add roleGroup: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            try
            {
                var roleGroup = await _context.RoleGroup.FirstOrDefaultAsync(x => x.Id == id);
                if (roleGroup == null) return false;
                _context.RoleGroup.Remove(roleGroup);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to delete roleGroup: {ex.Message}");
                throw;
            }
        }

        public Task<IQueryable<RoleGroup>> GetAllRoleGroupAsync()
        {
            try
            {
                return Task.FromResult(_context.RoleGroup.AsNoTracking().AsQueryable());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all room rentals");
                throw;
            }
        }

        public async Task<RoleGroup?> GetByIdAsync(long id)
        {
            try
            {
                return await _context.RoleGroup.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get roleGroup by id: {ex.Message}");
                throw;
            }

        }

        public async Task<bool> UpdateAsync(RoleGroup roleGroup)
        {
            try
            {
                var data = _context.RoleGroup.FirstOrDefault(x => x.Id == roleGroup.Id);
                if (data == null) return false;
                _context.RoleGroup.Update(roleGroup);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update roleGroup: {ex.Message}");
                throw;
            }

        }
    }
}
