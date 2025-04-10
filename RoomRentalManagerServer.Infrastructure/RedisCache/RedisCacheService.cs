using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RoomRentalManagerServer.Domain.Interfaces.RedisCache;
using StackExchange.Redis;

namespace RoomRentalManagerServer.Infrastructure.RedisCache
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDatabase _database;

        public RedisCacheService(IConfiguration configuration)
        {
            var redis = ConnectionMultiplexer.Connect(configuration["Redis:ConnectionString"]);
            _database = redis.GetDatabase();
        }

        public async Task SetAsync<T>(string key, List<T> value, TimeSpan? expiry = null)
        {
            var json = JsonConvert.SerializeObject(value);
            await _database.StringSetAsync(key, json, expiry);
        }

        public async Task<List<T>> GetAsync<T>(string key)
        {
            var json = await _database.StringGetAsync(key);
            if (string.IsNullOrEmpty(json))
            {
                return new List<T>();
            }
            else
            {
                return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
            }    
            
        }
    }
}
