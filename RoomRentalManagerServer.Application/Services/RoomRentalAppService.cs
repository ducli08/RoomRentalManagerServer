using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Application.Model.RoomRentalsModel.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomRentalManagerServer.Application.Services
{
    public class RoomRentalAppService : IRoomRentalAppService
    {
        public Task<RoomRentalDto> CreateOrEditRoomRentalAsync(CreateOrEditRoomRentalDto input)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteRoomRentalAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<List<RoomRentalDto>> GetAllRoomRentalAsync()
        {
            throw new NotImplementedException();
        }

        public Task<RoomRentalDto> GetRoomRentalByIdAsync(long id)
        {
            throw new NotImplementedException();
        }
    }
}
