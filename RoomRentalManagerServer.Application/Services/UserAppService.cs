using AutoMapper;
using Microsoft.Extensions.Logging;
using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Application.Model.UsersModel.Dto;
using RoomRentalManagerServer.Domain.Interfaces.UserInterfaces;
using RoomRentalManagerServer.Domain.ModelEntities.User;
using RoomRentalManagerServer.Infrastructure.Data;

namespace RoomRentalManagerServer.Application.Services
{
    public class UserAppService : IUserAppService
    {
        private readonly ILogger<UserAppService> _logger;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        public UserAppService(ILogger<UserAppService> logger,IUserRepository userRepository,IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _userRepository = userRepository;
        }
        public async Task<bool> CreateOrEditUserAsync(CreateOrEditUserDto input)
        {
            var action = input.Id != null ? "Edit" : "Create";
            var res = true;
            try
            {
                var user = _mapper.Map<Users>(input);
                if (input.Id != null)
                {
                    await _userRepository.UpdateAsync(user);
                }
                else
                {
                    await _userRepository.AddAsync(user);
                }
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to {action}: {ex.Message}");
                throw;
            }
            
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
