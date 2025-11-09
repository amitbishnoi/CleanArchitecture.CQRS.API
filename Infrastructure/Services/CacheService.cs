using Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T Get<T>(string key)
        {
            if (_cache.TryGetValue(key, out T? value) && value is not null)
            {
                return value;
            }
            else
            {
                // Return default value for T if not found or null
                return default!;
            }
        }

        public void Set<T>(string key, T value, TimeSpan duration)
        {
            _cache.Set(key, value, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = duration
            });
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
