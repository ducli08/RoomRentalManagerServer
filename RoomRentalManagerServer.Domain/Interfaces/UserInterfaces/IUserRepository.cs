using RoomRentalManagerServer.Domain.ModelEntities.User;

namespace RoomRentalManagerServer.Domain.Interfaces.UserInterfaces
{
    public interface IUserRepository
    {
        Task<Users> GetByIdAsync(long id);
        Task<IEnumerable<Users>> GetAllAsync();
        Task AddAsync(Users user);
        Task UpdateAsync(Users user);
        Task DeleteAsync(long id);
    }
}
