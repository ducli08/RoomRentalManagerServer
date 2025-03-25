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
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Users>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Users> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Users user)
        {
            throw new NotImplementedException();
        }
    }
}
