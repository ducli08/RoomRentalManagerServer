using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoomRentalManagerServer.Domain.Interfaces.RoomRentalInterfaces;
using RoomRentalManagerServer.Domain.ModelEntities.RoomRentals;
using RoomRentalManagerServer.Infrastructure.Data;

namespace RoomRentalManagerServer.Infrastructure.Repositories.RoomRentalRepositories
{
    public class RoomRentalRepository : IRoomRentalRepository
    {
        private readonly RoomRentalManagerServerDbContext _context;
        private readonly ILogger<RoomRentalRepository> _logger;
        public RoomRentalRepository(RoomRentalManagerServerDbContext context, ILogger<RoomRentalRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task AddAsync(RoomRental roomRental)
        {
            try
            {
                await _context.RoomRentals.AddAsync(roomRental);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to add room rental: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(long id)
        {
            try
            {
                var roomRental = await _context.RoomRentals.FirstOrDefaultAsync(x => x.Id == id);
                ArgumentNullException.ThrowIfNull(roomRental);
                _context.RoomRentals.Remove(roomRental);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to delete roomRental: {ex.Message}");
                throw;
            }
        }

        public async Task<IQueryable<RoomRental>> GetAllRoomRentalAsync()
        {
            try
            {
                return _context.RoomRentals.AsQueryable().AsNoTracking();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all roomrentals: {ex.Message}");
                throw;
            }
        }

        public async Task<RoomRental?> GetRoomRetalById(long id)
        {
            try
            {
                return await _context.RoomRentals.AsQueryable().FirstOrDefaultAsync(x => x.Id.Equals(id));
            }
            catch (Exception)
            {
                _logger.LogError($"Failed to get room rental by id: {id}");
                throw;
            }
        }

        public async Task UpdateAsync(RoomRental roomRental)
        {
            try
            {
                var existRoomRental = await _context.RoomRentals.FirstOrDefaultAsync(x => x.Id == roomRental.Id);
                ArgumentNullException.ThrowIfNull(existRoomRental);
                existRoomRental.RoomNumber = roomRental.RoomNumber;
                existRoomRental.RoomType = roomRental.RoomType;
                existRoomRental.Price = roomRental.Price;
                existRoomRental.StatusRoom = roomRental.StatusRoom;
                existRoomRental.Note = roomRental.Note;
                existRoomRental.Area = roomRental.Area;
                existRoomRental.ImagesDescription = roomRental.ImagesDescription;
                existRoomRental.UpdatedDate = DateTime.Now;
                existRoomRental.LastUpdateUser = roomRental.LastUpdateUser;
                existRoomRental.CreatedDate = roomRental.CreatedDate;
                existRoomRental.CreatorUser = roomRental.CreatorUser;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update roomrental: {ex.Message}");
                throw;
            }
        }
    }
}
