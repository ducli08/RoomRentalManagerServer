using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Domain.Interfaces.ProvinceInterface;
using RoomRentalManagerServer.Domain.ModelEntities.Provinces;

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

        public async Task<List<Province>> GetAllProvincesAsync()
        {
            try
            {
                var provincesQuery = await _provinceRepository.GetAllQueryAsync();
                return await provincesQuery.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all provinces");
                throw;
            }
        }
    }
}
