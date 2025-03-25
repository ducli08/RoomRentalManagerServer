using RoomRentalManagerServer.Application.Model.UsersModel.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomRentalManagerServer.Application.Interfaces
{
    public interface IUserAppService
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(long id);
        Task<UserDto> CreateOrEditUserAsync(CreateOrEditUserDto input);
        Task<bool> DeleteUserAsync(long id);
    }
}
