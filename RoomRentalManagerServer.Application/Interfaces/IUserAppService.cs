using RoomRentalManagerServer.Application.Common;
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
        Task<PagedResultDto<UserDto>> GetAllUsersAsync(PagedRequestDto<UserFilterDto> pagedRequestDto);
        Task<UserDto> GetUserByIdAsync(long id);
        Task<bool> CreateOrEditUserAsync(CreateOrEditUserDto input);
        Task<bool> DeleteUserAsync(long id);
        Task<UserDto> Authentication(string username, string password);
    }
}
