using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoomRentalManagerServer.Application.Common.CommonDto;
using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Application.Model.RoomRentalsModel.Dto;
using RoomRentalManagerServer.Application.Model.UsersModel.Dto;
using RoomRentalManagerServer.Domain.Interfaces.RoomRentalInterfaces;
using RoomRentalManagerServer.Domain.ModelEntities.RoomRentals;
using RoomRentalManagerServer.Domain.ModelEntities.User;

namespace RoomRentalManagerServer.Application.Services
{
    public class RoomRentalAppService : IRoomRentalAppService
    {
        private readonly ILogger<RoomRentalAppService> _logger;
        private readonly IMapper _mapper;
        private readonly IRoomRentalRepository _roomRentalRepository;
        private readonly ICurrentUserAppService _currentUserAppService;
        public RoomRentalAppService(ILogger<RoomRentalAppService> logger, IRoomRentalRepository roomRentalRepository, IMapper mapper, ICurrentUserAppService currentUserAppService)
        {
            _logger = logger;
            _mapper = mapper;
            _roomRentalRepository = roomRentalRepository;
            _currentUserAppService = currentUserAppService;
        }
        public async Task<bool> CreateOrEditRoomRentalAsync(CreateOrEditRoomRentalDto createOrEditRoomRentalDto)
        {
            var action = createOrEditRoomRentalDto.Id != null ? "Edit" : "Create";
            var res = true;
            try
            {
                var roomRental = _mapper.Map<RoomRental>(createOrEditRoomRentalDto);
                if (createOrEditRoomRentalDto.Id != null)
                {
                    await UpdateRoomRentalAsync(roomRental);
                }
                else
                {
                    await AddRoomRentalAsync(roomRental);
                }
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to {action}: {ex.Message}");
                throw;
            }
            throw new NotImplementedException();
        }

        public async Task DeleteRoomRentalAsync(long id)
        {
            try
            {
                await _roomRentalRepository.DeleteAsync(id);
            }
            catch (Exception)
            {

                throw;
            }
            throw new NotImplementedException();
        }

        public async Task<PagedResultDto<RoomRentalDto>> GetAllRoomRentalAsync(PagedRequestDto<RoomRentalFilterDto> pagedRequestRoomRentalDto)
        {
            try
            {
                var queryGetAllRoomRental = await _roomRentalRepository.GetAllRoomRentalAsync();
                if (!string.IsNullOrEmpty(pagedRequestRoomRentalDto.SortOrder) && !string.IsNullOrEmpty(pagedRequestRoomRentalDto.SortBy))
                {
                    queryGetAllRoomRental = pagedRequestRoomRentalDto.SortOrder == "desc"
                    ? queryGetAllRoomRental.OrderByDescending(x => EF.Property<object>(x, pagedRequestRoomRentalDto.SortBy))
                    : queryGetAllRoomRental.OrderBy(x => EF.Property<object>(x, pagedRequestRoomRentalDto.SortBy));
                }
                var total = queryGetAllRoomRental.Count();
                var roomRentals = await queryGetAllRoomRental.Skip((pagedRequestRoomRentalDto.Page - 1) * pagedRequestRoomRentalDto.PageSize).Take(pagedRequestRoomRentalDto.PageSize).ToListAsync();
                var roomRentalDtos = roomRentals.Select(user => _mapper.Map<RoomRentalDto>(user)).ToList();
                return new PagedResultDto<RoomRentalDto>(roomRentalDtos, total);
            }
            catch (Exception)
            {

                throw;
            }
            throw new NotImplementedException();
        }

        public async Task<RoomRentalDto> GetRoomRentalByIdAsync(long id)
        {
            throw new NotImplementedException();
        }
        public async Task<RoomRentalDto> AddRoomRentalAsync(RoomRental roomRental)
        {
            try
            {
                if (!_currentUserAppService.IsAuthenticated)
                {
                    throw new UnauthorizedAccessException("User is not authenticated.");
                }
                var userName = _currentUserAppService.UserName ?? throw new InvalidOperationException("User is null.");
                roomRental.CreatorUser = userName;
                roomRental.LastUpdateUser = userName;
                roomRental.CreatedDate = roomRental.UpdatedDate = DateTime.UtcNow;
                await _roomRentalRepository.AddAsync(roomRental);
                return _mapper.Map<RoomRentalDto>(roomRental);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to add user: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateRoomRentalAsync(RoomRental roomRental)
        {
            try
            {
                roomRental.LastUpdateUser = _currentUserAppService.UserName ?? throw new InvalidOperationException("User is null.");
                await _roomRentalRepository.UpdateAsync(roomRental);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update roomrental: {ex.Message}");
                throw;
            }
        }

        public async Task<List<RoomRental>> GetAllRoomRentalForSelectListItem()
        {
            try
            {
                var queryGetAllRoomRental = await _roomRentalRepository.GetAllRoomRentalAsync();
                return queryGetAllRoomRental.ToList();
            }
            catch (Exception)
            {
                _logger.LogError("Failed to get all room rentals for select list item.");
                throw;
            }
        }

        
    }
}
