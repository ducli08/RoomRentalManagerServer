using AutoMapper;
using Microsoft.Extensions.Logging;
using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Domain.Interfaces.ProvinceInterface;

namespace RoomRentalManagerServer.Application.Services
{
    public class ProvinceAppService : IProvinceAppService
    {
        private readonly ILogger<ProvinceAppService> _logger;
        private readonly IMapper _mapper;
        private readonly IProvinceRepository _provinceRepository;
        public ProvinceAppService(ILogger<ProvinceAppService> logger, IProvinceRepository provinceRepository, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _provinceRepository = provinceRepository;
        }
    }
}
