using RoomRentalManagerServer.Domain.ModelEntities.RoomRentals;
using RoomRentalManagerServer.Domain.ModelEntities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomRentalManagerServer.Domain.Interfaces.RoomRentalInterfaces
{
    public interface IRoomRentalRepository
    {
        Task<IEnumerable<RoomRental>> GetAllRoomRentalAsync();
        Task<RoomRental> GetRoomRetalById(long id);
        Task AddAsync(RoomRental roomRental);
        Task UpdateAsync(RoomRental roomRental);
        Task DeleteAsync(long id);
    }
}
