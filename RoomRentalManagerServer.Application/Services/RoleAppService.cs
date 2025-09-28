using AutoMapper;
using Microsoft.Extensions.Logging;
using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Application.Model.Roles.Dto;
using RoomRentalManagerServer.Domain.Interfaces.RoleInterfaces;

namespace RoomRentalManagerServer.Application.Services
{
    public class RoleAppService : IRoleAppService
    {
        private readonly ILogger<RoleAppService> _logger;
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;
        public RoleAppService(ILogger<RoleAppService> logger, IRoleRepository roleRepository, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _roleRepository = roleRepository;
        }

        public async Task<List<RoleDto>> GetAllRoleAsync()
        {
            try
            {
                var roles = await _roleRepository.GetAllQueryAsync();
                return _mapper.Map<List<RoleDto>>(roles.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get all roles: {ex.Message}");
                throw;
            }
        }
    }
}
