using Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace Infrastructure.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private static readonly HashSet<string> _cacheKeys = new();

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
                return default!;
            }
        }

        public void Set<T>(string key, T value, TimeSpan duration)
        {
            _cache.Set(key, value, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = duration
            });
            _cacheKeys.Add(key);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
            _cacheKeys.Remove(key);
        }

        /// <summary>
        /// Removes all cache entries matching a pattern (e.g., "users_*" removes all user-related cache)
        /// </summary>
        public void RemoveByPattern(string pattern)
        {
            var keysToRemove = _cacheKeys
                .Where(k => k.StartsWith(pattern, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var key in keysToRemove)
            {
                Remove(key);
            }
        }
    }
}
