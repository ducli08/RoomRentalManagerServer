using Microsoft.Extensions.Configuration;
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

        public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
        {
            await _database.StringSetAsync(key, value, expiry);
        }

        public async Task<string?> GetAsync(string key)
        {
            return await _database.StringGetAsync(key);
        }
    }
}
