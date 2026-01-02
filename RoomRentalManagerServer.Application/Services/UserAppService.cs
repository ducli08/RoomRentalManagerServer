using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RoomRentalManagerServer.Application.Common.CommonDto;
using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Application.Model.Login.Dto;
using RoomRentalManagerServer.Application.Model.UsersModel.Dto;
using RoomRentalManagerServer.Domain.Interfaces.UserInterfaces;
using RoomRentalManagerServer.Domain.ModelEntities.User;

namespace RoomRentalManagerServer.Application.Services
{
    public class UserAppService : IUserAppService
    {
        private readonly ILogger<UserAppService> _logger;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserAppService _currentUserAppService;
        private readonly ILocalFileStorageAppService _localFileStorageAppService;
        private readonly IConfiguration _configuration;
        public UserAppService(ILogger<UserAppService> logger, IUserRepository userRepository, IMapper mapper, ICurrentUserAppService currentUserAppService, ILocalFileStorageAppService localFileStorageAppService, IConfiguration configuration)
        {
            _logger = logger;
            _mapper = mapper;
            _userRepository = userRepository;
            _currentUserAppService = currentUserAppService;
            _localFileStorageAppService = localFileStorageAppService;
            _configuration = configuration;
        }

        public async Task<(List<string> Paths, List<string> Errors)> UploadAvatarAsync(List<IFormFile> avatar, string webRoot)
        {
            if (!_currentUserAppService.IsAuthenticated)
            {
                _logger.LogWarning("Unauthenticated user attempted to upload images");
                return (new List<string>(), new List<string> { "User is not authenticated." });
            }

            // delegate to file storage service; relative folder without leading slash
            var (paths, errors) = await _localFileStorageAppService.UploadFilesAsync(avatar, "uploads/avatar-images", webRoot);
            return (paths, errors);
        }

        public async Task<bool> CreateOrEditUserAsync(CreateOrEditUserDto input)
        {
            var action = input.Id != null ? "Edit" : "Create";
            var res = true;
            try
            {
                var user = _mapper.Map<Users>(input);
                if (input.Id != null && input.Id != 0)
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

        public async Task DeleteUserAsync(long id)
        {
            await _userRepository.DeleteAsync(id);
        }

        public async Task<PagedResultDto<UserDto>> GetAllUsersAsync(PagedRequestDto<UserFilterDto> pagedRequestDto)
        {
            try
            {
                var queryUser = await _userRepository.GetAllQueryAsync();
                if (!string.IsNullOrEmpty(pagedRequestDto.Filter.NameFilter))
                {
                    queryUser = queryUser.Where(x =>
                        EF.Functions.ILike(EF.Functions.Unaccent(x.Name), EF.Functions.Unaccent($"%{pagedRequestDto.Filter.NameFilter}%")) ||
                        EF.Functions.ILike(EF.Functions.Unaccent(x.Name), EF.Functions.Unaccent($"{pagedRequestDto.Filter.NameFilter}%")) ||
                        EF.Functions.ILike(EF.Functions.Unaccent(x.Name), EF.Functions.Unaccent($"{pagedRequestDto.Filter.NameFilter}%"))
                    );
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
                    queryUser = queryUser.Where(x =>
                        EF.Functions.ILike(EF.Functions.Unaccent(x.Address), EF.Functions.Unaccent($"%{pagedRequestDto.Filter.AddressFilter}%")) ||
                        EF.Functions.ILike(EF.Functions.Unaccent(x.Address), EF.Functions.Unaccent($"{pagedRequestDto.Filter.AddressFilter}%")) ||
                        EF.Functions.ILike(EF.Functions.Unaccent(x.Address), EF.Functions.Unaccent($"%{pagedRequestDto.Filter.AddressFilter}"))
                    );
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
                user.CreatedDate = user.UpdatedDate = DateTime.UtcNow;
                user.CreatorUser = user.LastUpdateUser = _currentUserAppService.UserName;
                await _userRepository.AddAsync(user);
                return _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to add user: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(Users user)
        {
            try
            {
                user.LastUpdateUser = _currentUserAppService.UserName;
                await _userRepository.UpdateAsync(user);
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
                if (user != null)
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

        public async Task<List<Users>> GetAllUserForSelectListItem()
        {
            try
            {
                var queryGetAllUser = await _userRepository.GetAllQueryAsync();
                return queryGetAllUser.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all user for selectListItem: {ex.Message}");
                throw;
            }
        }

        public async Task<UserDto> FindOrCreateGoogleUserAsync(GoogleTokenPayload googlePayload)
        {
            try
            {
                if (googlePayload == null || string.IsNullOrEmpty(googlePayload.Sub) || string.IsNullOrEmpty(googlePayload.Email))
                {
                    _logger.LogWarning("Invalid Google token payload");
                    throw new ArgumentException("Invalid Google token payload");
                }

                // First, try to find user by provider and providerId (Google sub)
                var user = await _userRepository.GetUserByProviderAsync("Google", googlePayload.Sub);
                
                if (user != null)
                {
                    // User exists with this Google account, return it
                    _logger.LogInformation($"Found existing Google user: {user.Email}");
                    return _mapper.Map<UserDto>(user);
                }

                // If not found by provider, check by email (for auto-linking)
                var userByEmail = await _userRepository.GetUserByEmail(googlePayload.Email);
                if (userByEmail != null)
                {
                    // User exists with this email but not linked to Google, link it
                    _logger.LogInformation($"Linking existing user {userByEmail.Email} to Google account");
                    userByEmail.Provider = "Google";
                    userByEmail.ProviderId = googlePayload.Sub;
                    if (!string.IsNullOrEmpty(googlePayload.Picture))
                    {
                        userByEmail.Avatar = googlePayload.Picture;
                    }
                    if (!string.IsNullOrEmpty(googlePayload.Name) && string.IsNullOrEmpty(userByEmail.Name))
                    {
                        userByEmail.Name = googlePayload.Name;
                    }
                    userByEmail.UpdatedDate = DateTime.UtcNow;
                    userByEmail.LastUpdateUser = "Google";
                    await _userRepository.UpdateAsync(userByEmail);
                    return _mapper.Map<UserDto>(userByEmail);
                }

                // User doesn't exist, create new user
                _logger.LogInformation($"Creating new Google user: {googlePayload.Email}");
                var defaultRoleGroupId = _configuration["Google:DefaultRoleGroupId"];
                int roleGroupId = 0;
                if (!string.IsNullOrEmpty(defaultRoleGroupId) && int.TryParse(defaultRoleGroupId, out int parsedRoleId))
                {
                    roleGroupId = parsedRoleId;
                }

                var newUser = new Users
                {
                    Email = googlePayload.Email,
                    Name = googlePayload.Name ?? googlePayload.Email,
                    Provider = "Google",
                    ProviderId = googlePayload.Sub,
                    Avatar = googlePayload.Picture,
                    RoleGroupId = roleGroupId,
                    Password = Guid.NewGuid().ToString(), // Placeholder password for Google users (never used)
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    CreatorUser = "Google",
                    LastUpdateUser = "Google"
                };

                // For Google users, we don't hash the password since it's just a placeholder
                // We'll add directly to repository without going through AddAsync which hashes
                newUser.CreatedDate = newUser.UpdatedDate = DateTime.UtcNow;
                await _userRepository.AddAsync(newUser);
                return _mapper.Map<UserDto>(newUser);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to find or create Google user: {ex.Message}");
                throw;
            }
        }
    }
}
