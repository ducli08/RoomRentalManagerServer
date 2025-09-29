using AutoMapper;
using Microsoft.Extensions.Logging;
using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Application.Model.PermissionModel.Dto;
using RoomRentalManagerServer.Application.Model.Roles.Dto;
using RoomRentalManagerServer.Domain.Interfaces;
using RoomRentalManagerServer.Domain.Interfaces.RoleInterfaces;

namespace RoomRentalManagerServer.Application.Services
{
    public class RoleAppService : IRoleAppService
    {
        private readonly ILogger<RoleAppService> _logger;
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IPermissionRepository _permissionRepository;
        public RoleAppService(ILogger<RoleAppService> logger, IRoleRepository roleRepository,
            IMapper mapper, IRolePermissionRepository rolePermissionRepository, IPermissionRepository permissionRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _roleRepository = roleRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _permissionRepository = permissionRepository;
        }

        public async Task<List<RoleDto>> GetAllRoleAsync()
        {
            try
            {
                var query = await _roleRepository.GetAllQueryAsync();
                var roleIds = query.Select(r => r.Id).ToList();
                var rolePermissions = await _rolePermissionRepository.GetAllRolePermissionByListRoleIdAsync(roleIds);
                var permissionIds = rolePermissions.Select(x => x.PermissionId).Distinct().ToList();
                var permissions = await _permissionRepository.GetAllPermissionByListIdAsync(permissionIds);
                var result = query.Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Permissions = rolePermissions
                        .Where(rp => rp.RoleId == r.Id)
                        .Select(rp => permissions.First(p => p.Id == rp.PermissionId))
                        .Select(p => new PermissionDto
                        {
                            Id = p.Id,
                            Name = p.Name
                        }).ToList()
                });
                return _mapper.Map<List<RoleDto>>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all roles: {ex.Message}");
                throw;
            }
        }
    }
}
