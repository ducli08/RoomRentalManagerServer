using RoomRentalManagerServer.Domain.ModelEntities.User;

namespace RoomRentalManagerServer.Domain.Interfaces.UserInterfaces
{
    public interface IUserRepository
    {
        Task<Users?> GetByIdAsync(long id);
        Task<IQueryable<Users>> GetAllQueryAsync();
        Task AddAsync(Users user);
        Task UpdateAsync(Users user);
        Task DeleteAsync(long id);
        Task<Users> GetUserByEmail(string email);
    }
}
