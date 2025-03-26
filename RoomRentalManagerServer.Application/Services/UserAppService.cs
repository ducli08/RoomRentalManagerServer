using Microsoft.Extensions.Logging;
using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Application.Model.UsersModel.Dto;

namespace RoomRentalManagerServer.Application.Services
{
    public class UserAppService : IUserAppService
    {
        private readonly ILogger<UserAppService> _logger;
        public UserAppService(ILogger<UserAppService> logger)
        {
            _logger = logger;
        }
        public async Task<UserDto> CreateOrEditUserAsync(CreateOrEditUserDto input)
        {
            _logger.LogInformation($"Start Create or Update User");
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteUserAsync(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<UserDto> GetUserByIdAsync(long id)
        {
            throw new NotImplementedException();
        }
    }
}
