using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoomRentalManagerServer.Application.Common;
using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Application.Model.Login.Dto;
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

        public async Task<PagedResultDto<UserDto>> GetAllUsersAsync(PagedRequestDto<UserFilterDto> pagedRequestDto)
        {
            try
            {
                var queryUser = await _userRepository.GetAllQueryAsync();
                if (!string.IsNullOrEmpty(pagedRequestDto.Filter.NameFilter))
                {
                    queryUser = queryUser.Where(x => x.Name.Equals(pagedRequestDto.Filter.NameFilter));
                }
                if (!string.IsNullOrEmpty(pagedRequestDto.Filter.ProvinceCodeFilter))
                {
                    queryUser = queryUser.Where(x => x.ProvinceCode.Equals(pagedRequestDto.Filter.ProvinceCodeFilter));
                }
                if (!string.IsNullOrEmpty(pagedRequestDto.Filter.DistrictCodeFilter))
                {
                    queryUser = queryUser.Where(x => x.DistrictCode.Equals(pagedRequestDto.Filter.DistrictCodeFilter));
                }
                if (!string.IsNullOrEmpty(pagedRequestDto.Filter.WardCodeFilter))
                {
                    queryUser = queryUser.Where(x => x.WardCode.Equals(pagedRequestDto.Filter.WardCodeFilter));
                }
                if (!string.IsNullOrEmpty(pagedRequestDto.Filter.AddressFilter))
                {
                    queryUser = queryUser.Where(x => x.Address.Equals(pagedRequestDto.Filter.AddressFilter));
                }
                if (!string.IsNullOrEmpty(pagedRequestDto.Filter.EmailFilter))
                {
                    queryUser = queryUser.Where(x => x.Email.Equals(pagedRequestDto.Filter.EmailFilter));
                }
                if (!string.IsNullOrEmpty(pagedRequestDto.Filter.IDCardFilter))
                {
                    queryUser = queryUser.Where(x => x.IDCard.Equals(pagedRequestDto.Filter.IDCardFilter));
                }
                if (pagedRequestDto.Filter.DateOfBirth != null)
                {
                    queryUser = queryUser.Where(x => x.DateOfBirth == pagedRequestDto.Filter.DateOfBirth);
                }
                if (!string.IsNullOrEmpty(pagedRequestDto.SortOrder) && !string.IsNullOrEmpty(pagedRequestDto.SortBy))
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
                var hasher = new PasswordHasher<Users>();
                user.Password = hasher.HashPassword(user, user.Password); // Hash the password before saving
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

        public async Task<UserDto> Authentication(string username, string password)
        {
            try
            {
                var user = await _userRepository.GetUserByEmail(username);
                if(user != null)
                {
                    var hasher = new PasswordHasher<Users>();
                    var result = hasher.VerifyHashedPassword(user, user.Password, password);
                    if (result == PasswordVerificationResult.Success)
                    {
                        return _mapper.Map<UserDto>(user);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
            
        }
    }
}
