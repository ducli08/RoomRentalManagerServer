using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoomRentalManagerServer.Application.Common.CommonDto;
using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Application.Model.RoomRentalsModel.Dto;
using RoomRentalManagerServer.Domain.Interfaces.RoomRentalInterfaces;
using RoomRentalManagerServer.Domain.ModelEntities.RoomRentals;
using System.IO;
using Microsoft.AspNetCore.Http;

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

        public async Task<(List<string> Paths, List<string> Errors)> UploadImageDescriptionAsync(List<IFormFile> uploadImages, string webRoot)
        {
            var errors = new List<string>();

            if (!_currentUserAppService.IsAuthenticated)
            {
                _logger.LogWarning("Unauthenticated user attempted to upload images");
                errors.Add("User is not authenticated.");
                return (new List<string>(), errors);
            }

            var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
                ".jpg", ".jpeg", ".png", ".gif" };
            const long maxFileSize = 5 * 1024 * 1024; // 5 MB

            var savedPaths = new List<string>();

            var uploadsRootFolder = Path.Combine(webRoot ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", "room-images");
            if (!Directory.Exists(uploadsRootFolder))
                Directory.CreateDirectory(uploadsRootFolder);

            foreach (var file in uploadImages)
            {
                if (file == null)
                    continue;

                if (file.Length == 0)
                {
                    errors.Add($"File {file?.FileName} is empty.");
                    continue;
                }

                if (file.Length > maxFileSize)
                {
                    errors.Add($"File {file.FileName} exceeds the allowed size of {maxFileSize} bytes.");
                    continue;
                }

                var ext = Path.GetExtension(file.FileName);
                if (string.IsNullOrEmpty(ext) || !allowedExtensions.Contains(ext))
                {
                    errors.Add($"File {file.FileName} has an invalid extension.");
                    continue;
                }

                var uniqueFileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsRootFolder, uniqueFileName);

                try
                {
                    await using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    var relativePath = $"/uploads/room-images/{uniqueFileName}";
                    savedPaths.Add(relativePath);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to save uploaded image {FileName}", file.FileName);
                    errors.Add($"Failed to save {file.FileName}: {ex.Message}");
                }
            }

            return (savedPaths, errors);
        }

        public async Task<bool> CreateOrEditRoomRentalAsync(CreateOrEditRoomRentalDto createOrEditRoomRentalDto)
        {
            if (createOrEditRoomRentalDto == null)
                return false;

            var isUpdate = createOrEditRoomRentalDto.Id.HasValue && createOrEditRoomRentalDto.Id.Value > 0;
            var action = isUpdate ? "Edit" : "Create";

            try
            {
                var roomRental = _mapper.Map<RoomRental>(createOrEditRoomRentalDto);

                // Normalize/parse string fields coming from DTO
                if (!string.IsNullOrWhiteSpace(createOrEditRoomRentalDto.RoomNumber))
                {
                    if (int.TryParse(createOrEditRoomRentalDto.RoomNumber, out var rn))
                        roomRental.RoomNumber = rn;
                }

                if (!string.IsNullOrWhiteSpace(createOrEditRoomRentalDto.Price))
                {
                    if (double.TryParse(createOrEditRoomRentalDto.Price, out var price))
                        roomRental.Price = price;
                }

                if (!string.IsNullOrWhiteSpace(createOrEditRoomRentalDto.Area))
                {
                    if (double.TryParse(createOrEditRoomRentalDto.Area, out var area))
                        roomRental.Area = area;
                }

                if (isUpdate)
                {
                    await UpdateRoomRentalAsync(roomRental);
                }
                else
                {
                    await AddRoomRentalAsync(roomRental);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to {Action} room rental", action);
                return false;
            }
        }

        public async Task DeleteRoomRentalAsync(long id)
        {
            try
            {
                var roomRental = await _roomRentalRepository.GetRoomRetalById(id);
                if (roomRental == null)
                {
                    _logger.LogWarning("Room rental with id {Id} not found when attempting delete", id);
                    throw new KeyNotFoundException($"Room rental with id {id} not found.");
                }

                // Delete image files from disk if present. Don't fail delete if file deletion fails.
                try
                {
                    var webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    if (roomRental.ImagesDescription != null && roomRental.ImagesDescription.Any())
                    {
                        foreach (var imgPath in roomRental.ImagesDescription)
                        {
                            if (string.IsNullOrWhiteSpace(imgPath))
                                continue;

                            var relative = imgPath.TrimStart('~').TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                            var fullPath = Path.Combine(webRoot, relative);

                            try
                            {
                                if (File.Exists(fullPath))
                                {
                                    File.Delete(fullPath);
                                    _logger.LogInformation("Deleted image file {FilePath} for room rental {Id}", fullPath, id);
                                }
                                else
                                {
                                    _logger.LogWarning("Image file not found when deleting room rental {Id}: {FilePath}", id, fullPath);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Failed to delete image file {FilePath} for room rental {Id}", fullPath, id);
                                // continue with other files
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error while attempting to delete image files for room rental {Id}", id);
                }

                await _roomRentalRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete room rental with id {Id}", id);
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
                if (!string.IsNullOrEmpty(pagedRequestRoomRentalDto.Filter?.Note))
                {
                    var pattern = $"%{pagedRequestRoomRentalDto.Filter.Note}%";
                    queryGetAllRoomRental = queryGetAllRoomRental.Where(x =>
                        EF.Functions.ILike(EF.Functions.Unaccent(x.Note), EF.Functions.Unaccent(pattern))
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

                var total = await queryGetAllRoomRental.CountAsync();
                var roomRentals = await queryGetAllRoomRental.Skip((pagedRequestRoomRentalDto.Page - 1) * pagedRequestRoomRentalDto.PageSize).Take(pagedRequestRoomRentalDto.PageSize).ToListAsync();
                var roomRentalDtos = roomRentals.Select(r => _mapper.Map<RoomRentalDto>(r)).ToList();
                return new PagedResultDto<RoomRentalDto>(roomRentalDtos, total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all room rentals");
                throw;
            }
        }

        public async Task<RoomRentalDto?> GetRoomRentalByIdAsync(long id)
        {
            try
            {
                var roomRental = await _roomRentalRepository.GetRoomRetalById(id);
                if (roomRental == null)
                {
                    return null;
                }
                return _mapper.Map<RoomRentalDto>(roomRental);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get room rental by ID {Id}", id);
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
                _logger.LogError(ex, "Failed to add room rental");
                throw;
            }
        }

        public async Task UpdateRoomRentalAsync(RoomRental roomRental)
        {
            try
            {
                roomRental.LastUpdateUser = _currentUserAppService.UserName ?? throw new InvalidOperationException("User is null.");
                roomRental.UpdatedDate = DateTime.UtcNow;
                await _roomRentalRepository.UpdateAsync(roomRental);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update room rental");
                throw;
            }
        }

        public async Task<List<RoomRental>> GetAllRoomRentalForSelectListItem()
        {
            try
            {
                var queryGetAllRoomRental = await _roomRentalRepository.GetAllRoomRentalAsync();
                return await queryGetAllRoomRental.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all room rentals for select list item.");
                throw;
            }
        }

        
    }
}
