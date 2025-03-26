using AutoMapper;
using Microsoft.Extensions.Logging;
using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Application.Model.UsersModel.Dto;
using RoomRentalManagerServer.Domain.ModelEntities.User;
using RoomRentalManagerServer.Infrastructure.Data;

namespace RoomRentalManagerServer.Application.Services
{
    public class UserAppService : IUserAppService
    {
        private readonly ILogger<UserAppService> _logger;
        private readonly RoomRentalManagerServerDbContext _context;
        private readonly IMapper _mapper;
        public UserAppService(ILogger<UserAppService> logger, RoomRentalManagerServerDbContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
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
                    await _context.Users.AddAsync(user);
                }
                else
                {
                    _context.Users.Update(user);
                }
                _context.SaveChanges();
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
