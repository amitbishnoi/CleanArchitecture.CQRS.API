using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace API.HealthChecks
{
    /// <summary>
    /// Custom health check for memory cache availability and performance
    /// </summary>
    public class CacheHealthCheck : IHealthCheck
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<CacheHealthCheck> _logger;

        public CacheHealthCheck(IMemoryCache cache, ILogger<CacheHealthCheck> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var testKey = "_health_check_test_key";
                var testValue = "test";

                // Test cache write
                _cache.Set(testKey, testValue, TimeSpan.FromSeconds(5));

                // Test cache read
                var cachedValue = _cache.Get<string>(testKey);

                if (cachedValue != testValue)
                {
                    return Task.FromResult(HealthCheckResult.Degraded("Cache read/write mismatch"));
                }

                // Cleanup
                _cache.Remove(testKey);

                var data = new Dictionary<string, object>
                {
                    { "CacheType", "InMemory" },
                    { "Status", "Operational" }
                };

                return Task.FromResult(HealthCheckResult.Healthy(
                    "Cache is healthy and operational",
                    data: data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache health check failed");
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    "Cache health check failed",
                    exception: ex));
            }
        }
    }
}
