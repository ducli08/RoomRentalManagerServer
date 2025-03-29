using Microsoft.EntityFrameworkCore;
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
        public UserRepository(RoomRentalManagerServerDbContext context) { 
            context = _context;
        }
        public async Task AddAsync(Users user)
        {
            var data = _context.Users.FirstOrDefault(x => x.Id == user.Id);
            if(data != null)
            {
                _context.Add(user);
            }
            await _context.SaveChangesAsync();
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(long id)
        {
            var data = _context.Users.FirstOrDefault(x => x.Id == id);
            if(data != null)
            {
                _context.Remove(data);
            }
            await _context.SaveChangesAsync();
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Users>> GetAllAsync()
        {
            var lstUser = await _context.Users.ToListAsync();
            return lstUser;
        }

        public async Task<Users> GetByIdAsync(long id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if(user != null)
            {
                return user;
            }
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Users user)
        {
            var data = _context.Users.FirstOrDefault(x => x.Id == user.Id);
            if(data != null)
            {
                _context.Update(user);
            }
            await _context.SaveChangesAsync();
        }
    }
}
