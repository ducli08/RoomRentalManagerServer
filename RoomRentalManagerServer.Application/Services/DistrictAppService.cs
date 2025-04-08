using AutoMapper;
using Microsoft.Extensions.Logging;
using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Domain.Interfaces.DistrictInterfaces;
using RoomRentalManagerServer.Domain.ModelEntities.Districts;

namespace RoomRentalManagerServer.Application.Services
{
    public class DistrictAppService : IDistrictAppService
    {
        private readonly ILogger<DistrictAppService> _logger;
        private readonly IMapper _mapper;
        private readonly IDistrictRepository _districtRepository;
        public DistrictAppService(ILogger<DistrictAppService> logger, IDistrictRepository districtRepository, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _districtRepository = districtRepository;
        }

        public async Task<IQueryable<District>> GetAllDistrictsAsync(string? provinceCode)
        {
            try
            {
                var districtsQuery = await _districtRepository.GetAllQueryAsync();
                if (!string.IsNullOrEmpty(provinceCode))
                {
                    districtsQuery = districtsQuery.Where(d => d.ProvinceCode == provinceCode);
                }
                return districtsQuery;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all districts");
                throw;
            }
        }
    }
}
