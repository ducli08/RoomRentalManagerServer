using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Domain.Interfaces.DistrictInterfaces;
using RoomRentalManagerServer.Domain.Interfaces.RedisCache;
using RoomRentalManagerServer.Domain.ModelEntities.Districts;

namespace RoomRentalManagerServer.Application.Services
{
    public class DistrictAppService : IDistrictAppService
    {
        private readonly ILogger<DistrictAppService> _logger;
        private readonly IMapper _mapper;
        private readonly IDistrictRepository _districtRepository;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IConfiguration _configuration;
        public DistrictAppService(ILogger<DistrictAppService> logger, IDistrictRepository districtRepository, IMapper mapper,
            IRedisCacheService redisCacheService, IConfiguration configuration)
        {
            _logger = logger;
            _mapper = mapper;
            _districtRepository = districtRepository;
            _redisCacheService = redisCacheService;
            _configuration = configuration;
        }

        public async Task<List<District>> GetAllDistrictsAsync(string? provinceCode)
        {
            try
            {
                var value = await _redisCacheService.GetAsync(_configuration["Redis:Keys:District"]);
                var districts = new List<District>();
                if (value != null)
                {
                    districts = Newtonsoft.Json.JsonConvert.DeserializeObject<List<District>>(value);
                }
                else
                {
                    var districtsQuery = await _districtRepository.GetAllQueryAsync();
                    districts = await districtsQuery.ToListAsync();
                    if (districts != null)
                    {
                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(districts);
                        await _redisCacheService.SetAsync(_configuration["Redis:Keys:District"], json, TimeSpan.FromMinutes(30));
                    }
                }
                if (!string.IsNullOrEmpty(provinceCode))
                {
                    districts = districts.FindAll(x => x.ProvinceCode == provinceCode);
                }
                return districts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all districts");
                throw;
            }
        }
    }
}
