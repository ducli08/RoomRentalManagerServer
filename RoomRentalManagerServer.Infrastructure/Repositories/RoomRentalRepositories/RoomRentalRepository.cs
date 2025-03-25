using RoomRentalManagerServer.Domain.Interfaces.RoomRentalInterfaces;
using RoomRentalManagerServer.Domain.ModelEntities.RoomRentals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomRentalManagerServer.Infrastructure.Repositories.RoomRentalRepositories
{
    public class RoomRentalRepository : IRoomRentalRepository
    {
        public Task AddAsync(RoomRental roomRental)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RoomRental>> GetAllRoomRentalAsync()
        {
            throw new NotImplementedException();
        }

        public Task<RoomRental> GetRoomRetalById(long id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(RoomRental roomRental)
        {
            throw new NotImplementedException();
        }
    }
}
