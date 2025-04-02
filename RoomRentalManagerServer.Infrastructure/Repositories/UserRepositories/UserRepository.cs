using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoomRentalManagerServer.Domain.Interfaces.UserInterfaces;
using RoomRentalManagerServer.Domain.ModelEntities.User;
using RoomRentalManagerServer.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomRentalManagerServer.Infrastructure.Repositories.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly RoomRentalManagerServerDbContext _context;
        private readonly ILogger<UserRepository> _logger;
        public UserRepository(RoomRentalManagerServerDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<Users> AddAsync(Users user)
        {
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to add user: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (user == null) return false;
                _context.Users.Remove(user);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to delete user: {ex.Message}");
                throw;
            }
        }

        public async Task<IQueryable<Users>> GetAllQueryAsync()
        {
            try
            {
                return _context.Users.AsQueryable().AsNoTracking();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all users: {ex.Message}");
                throw;
            }
        }

        public async Task<Users?> GetByIdAsync(long id)
        {
            try
            {
                return await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get user by id: {ex.Message}");
                throw;
            }
            
        }

        public async Task<bool> UpdateAsync(Users user)
        {
            try
            {
                var data = _context.Users.FirstOrDefault(x => x.Id == user.Id);
                if (data == null) return false;
                _context.Users.Update(user);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update user: {ex.Message}");
                throw;
            }
            
        }
    }
}
