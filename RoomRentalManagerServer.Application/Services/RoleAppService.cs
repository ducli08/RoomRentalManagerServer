using AutoMapper;
using Microsoft.Extensions.Logging;
using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Domain.Interfaces.RoleGroupInterfaces;
using RoomRentalManagerServer.Domain.Interfaces.RoleInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
