using RoomRentalManagerServer.Domain.ModelEntities.User;

namespace RoomRentalManagerServer.Domain.Interfaces.UserInterfaces
{
    public interface IUserRepository
    {
        Task<Users?> GetByIdAsync(long id);
        Task<IQueryable<Users>> GetAllQueryAsync();
        Task<Users> AddAsync(Users user);
        Task<bool> UpdateAsync(Users user);
        Task<bool> DeleteAsync(long id);
    }
}
