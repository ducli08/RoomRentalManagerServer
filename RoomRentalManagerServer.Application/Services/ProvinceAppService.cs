using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RoomRentalManagerServer.Application.Interfaces;
using RoomRentalManagerServer.Domain.Interfaces.ProvinceInterface;
using RoomRentalManagerServer.Domain.Interfaces.RedisCache;
using RoomRentalManagerServer.Domain.ModelEntities.Provinces;

namespace RoomRentalManagerServer.Application.Services
{
    public class ProvinceAppService : IProvinceAppService
    {
        private readonly ILogger<ProvinceAppService> _logger;
        private readonly IMapper _mapper;
        private readonly IProvinceRepository _provinceRepository;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IConfiguration _configuration;
        public ProvinceAppService(ILogger<ProvinceAppService> logger, IProvinceRepository provinceRepository, IMapper mapper,
            IRedisCacheService redisCacheService, IConfiguration configuration)
        {
            _logger = logger;
            _mapper = mapper;
            _provinceRepository = provinceRepository;
            _redisCacheService = redisCacheService;
            _configuration = configuration;
        }

        public async Task<List<Province>> GetAllProvincesAsync()
        {
            try
            {
                var value = await _redisCacheService.GetAsync(_configuration["Redis:Keys:Province"]);
                var provinces = new List<Province>();
                if (value != null)
                {
                    provinces = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Province>>(value);
                }
                else
                {
                    var provincesQuery = await _provinceRepository.GetAllQueryAsync();
                    provinces = await provincesQuery.ToListAsync();
                    if (provinces != null)
                    {
                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(provinces);
                        await _redisCacheService.SetAsync(_configuration["Redis:Keys:Province"], json, TimeSpan.FromMinutes(30));
                    }
                }
                return provinces;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all provinces");
                throw;
            }
        }
    }
}
