using RoomRentalManagerServer.Application.Model.RoomRentalsModel.Dto;
using RoomRentalManagerServer.Application.Model.UsersModel.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomRentalManagerServer.Application.Interfaces
{
    public interface IRoomRentalAppService
    {
        Task<List<RoomRentalDto>> GetAllRoomRentalAsync();
        Task<RoomRentalDto> GetRoomRentalByIdAsync(long id);
        Task<RoomRentalDto> CreateOrEditRoomRentalAsync(CreateOrEditRoomRentalDto input);
        Task<bool> DeleteRoomRentalAsync(long id);
    }
}
