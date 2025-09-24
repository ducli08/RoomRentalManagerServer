using RoomRentalManagerServer.Domain.ModelEntities.RoleGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomRentalManagerServer.Domain.Interfaces.RoleGroupInterfaces
{
    public interface IRoleGroupRepository
    {
        Task<RoleGroup> AddAsync(RoleGroup roleGroup);
        Task<bool> DeleteAsync(long id);
        Task<RoleGroup> GetByIdAsync(long id);
        Task<bool> UpdateAsync(RoleGroup roleGroup);
        Task<IQueryable<RoleGroup>> GetAllRoleGroupAsync();
    }
}
