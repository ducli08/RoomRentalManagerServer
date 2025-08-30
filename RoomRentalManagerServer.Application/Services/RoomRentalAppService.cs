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
            var action = createOrEditRoomRentalDto.Id != 0 ? "Edit" : "Create";
            var res = true;
            try
            {
                var roomRental = _mapper.Map<RoomRental>(createOrEditRoomRentalDto);
                if (createOrEditRoomRentalDto.Id != 0)
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
        }

        public async Task<PagedResultDto<RoomRentalDto>> GetAllRoomRentalAsync(PagedRequestDto<RoomRentalFilterDto> pagedRequestRoomRentalDto)
        {
            try
            {
                var queryGetAllRoomRental = await _roomRentalRepository.GetAllRoomRentalAsync();
                
                // Filter by RoomNumber
                if (!string.IsNullOrEmpty(pagedRequestRoomRentalDto.Filter?.RoomNumber))
                {
                    if (int.TryParse(pagedRequestRoomRentalDto.Filter.RoomNumber, out int roomNumber) && roomNumber > 0)
                    {
                        queryGetAllRoomRental = queryGetAllRoomRental.Where(x => x.RoomNumber == roomNumber);
                    }
                }
                
                // Filter by RoomType
                if (pagedRequestRoomRentalDto.Filter?.RoomType != 0)
                {
                    queryGetAllRoomRental = queryGetAllRoomRental.Where(x => x.RoomType == pagedRequestRoomRentalDto.Filter.RoomType);
                }
                
                // Filter by Price range
                if (!string.IsNullOrEmpty(pagedRequestRoomRentalDto.Filter?.PriceStart))
                {
                    if (double.TryParse(pagedRequestRoomRentalDto.Filter.PriceStart, out double priceStart) && priceStart >= 0)
                    {
                        queryGetAllRoomRental = queryGetAllRoomRental.Where(x => x.Price >= priceStart);
                    }
                }
                
                if (!string.IsNullOrEmpty(pagedRequestRoomRentalDto.Filter?.PriceEnd))
                {
                    if (double.TryParse(pagedRequestRoomRentalDto.Filter.PriceEnd, out double priceEnd) && priceEnd >= 0)
                    {
                        queryGetAllRoomRental = queryGetAllRoomRental.Where(x => x.Price <= priceEnd);
                    }
                }
                
                // Filter by StatusRoom
                if (pagedRequestRoomRentalDto.Filter?.StatusRoom != 0)
                {
                    queryGetAllRoomRental = queryGetAllRoomRental.Where(x => x.StatusRoom == pagedRequestRoomRentalDto.Filter.StatusRoom);
                }

                // Filter by Note (contains search)
                if (!string.IsNullOrEmpty(pagedRequestRoomRentalDto.Filter.Note))
                {
                    queryGetAllRoomRental = queryGetAllRoomRental.Where(x =>
                        EF.Functions.ILike(EF.Functions.Unaccent(x.Note), EF.Functions.Unaccent($"%{pagedRequestRoomRentalDto.Filter.Note}%")) ||
                        EF.Functions.ILike(EF.Functions.Unaccent(x.Note), EF.Functions.Unaccent($"{pagedRequestRoomRentalDto.Filter.Note}%")) ||
                        EF.Functions.ILike(EF.Functions.Unaccent(x.Note), EF.Functions.Unaccent($"{pagedRequestRoomRentalDto.Filter.Note}%"))
                    );
                }

                // Filter by Area range
                if (!string.IsNullOrEmpty(pagedRequestRoomRentalDto.Filter?.AreaStart))
                {
                    if (double.TryParse(pagedRequestRoomRentalDto.Filter.AreaStart, out double areaStart) && areaStart >= 0)
                    {
                        queryGetAllRoomRental = queryGetAllRoomRental.Where(x => x.Area >= areaStart);
                    }
                }
                
                if (!string.IsNullOrEmpty(pagedRequestRoomRentalDto.Filter?.AreaEnd))
                {
                    if (double.TryParse(pagedRequestRoomRentalDto.Filter.AreaEnd, out double areaEnd) && areaEnd >= 0)
                    {
                        queryGetAllRoomRental = queryGetAllRoomRental.Where(x => x.Area <= areaEnd);
                    }
                }
                
                // Filter by CreatedDate
                if (pagedRequestRoomRentalDto.Filter?.CreatedDate.HasValue == true)
                {
                    var filterDate = pagedRequestRoomRentalDto.Filter.CreatedDate.Value.Date;
                    queryGetAllRoomRental = queryGetAllRoomRental.Where(x => x.CreatedDate.Date == filterDate);
                }
                
                // Filter by UpdatedDate
                if (pagedRequestRoomRentalDto.Filter?.UpdatedDate.HasValue == true)
                {
                    var filterDate = pagedRequestRoomRentalDto.Filter.UpdatedDate.Value.Date;
                    queryGetAllRoomRental = queryGetAllRoomRental.Where(x => x.UpdatedDate.Date == filterDate);
                }
                
                // Filter by CreatorUser (contains search)
                if (!string.IsNullOrEmpty(pagedRequestRoomRentalDto.Filter?.CreatorUser))
                {
                    queryGetAllRoomRental = queryGetAllRoomRental.Where(x => x.CreatorUser != null && x.CreatorUser.Contains(pagedRequestRoomRentalDto.Filter.CreatorUser));
                }
                
                // Filter by LastUpdateUser (contains search)
                if (!string.IsNullOrEmpty(pagedRequestRoomRentalDto.Filter?.LastUpdateUser))
                {
                    queryGetAllRoomRental = queryGetAllRoomRental.Where(x => x.LastUpdateUser != null && x.LastUpdateUser.Contains(pagedRequestRoomRentalDto.Filter.LastUpdateUser));
                }
                
                // Apply sorting
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
        }

        public async Task<RoomRentalDto> GetRoomRentalByIdAsync(long id)
        {
            try
            {
                var roomRental = await _roomRentalRepository.GetRoomRetalById(id);
                if (roomRental == null)
                {
                    throw new ArgumentException($"Room rental with ID {id} not found.");
                }
                return _mapper.Map<RoomRentalDto>(roomRental);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get room rental by ID {id}: {ex.Message}");
                throw;
            }
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
