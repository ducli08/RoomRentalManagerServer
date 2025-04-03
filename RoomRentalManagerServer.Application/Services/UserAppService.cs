using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoomRentalManagerServer.Application.Common;
using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Application.Model.UsersModel.Dto;
using RoomRentalManagerServer.Domain.Interfaces.UserInterfaces;
using RoomRentalManagerServer.Domain.ModelEntities.User;
using System.Linq;

namespace RoomRentalManagerServer.Application.Services
{
    public class UserAppService : IUserAppService
    {
        private readonly ILogger<UserAppService> _logger;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        public UserAppService(ILogger<UserAppService> logger, IUserRepository userRepository, IMapper mapper)
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
                    await UpdateAsync(user);
                }
                else
                {
                    await AddAsync(user);
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
            return await _userRepository.DeleteAsync(id);
        }

        public async Task<PagedResultDto<UserDto>> GetAllUsersAsync(PagedRequestDto pagedRequestDto)
        {
            try
            {
                var queryUser = await _userRepository.GetAllQueryAsync();
                if (!string.IsNullOrEmpty(pagedRequestDto.Search))
                {
                    queryUser = queryUser.Where(x => x.Name.Contains(pagedRequestDto.Search));
                }
                if(!string.IsNullOrEmpty(pagedRequestDto.SortOrder) && !string.IsNullOrEmpty(pagedRequestDto.SortBy))
                {
                    queryUser = pagedRequestDto.SortOrder == "desc"
                    ? queryUser.OrderByDescending(x => EF.Property<object>(x, pagedRequestDto.SortBy))
                    : queryUser.OrderBy(x => EF.Property<object>(x, pagedRequestDto.SortBy));
                } 
                var total = queryUser.Count();
                var lstUser = await queryUser.Skip((pagedRequestDto.Page - 1) * pagedRequestDto.PageSize).Take(pagedRequestDto.PageSize).ToListAsync();
                var lstUserDto = lstUser.Select(user => _mapper.Map<UserDto>(user)).ToList();
                return new PagedResultDto<UserDto>(lstUserDto, total);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all users: {ex.Message}");
                throw;
            }
        }

        public async Task<UserDto> GetUserByIdAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> AddAsync(Users user)
        {
            try
            {
                await _userRepository.AddAsync(user);
                return _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to add user: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Users user)
        {
            try
            {
                return await _userRepository.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update user: {ex.Message}");
                throw;
            }
        }
    }
}
