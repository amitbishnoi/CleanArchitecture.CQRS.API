using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace API.HealthChecks
{
    /// <summary>
    /// Custom health check for database connectivity and performance
    /// </summary>
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<DatabaseHealthCheck> _logger;

        public DatabaseHealthCheck(ApplicationDbContext dbContext, ILogger<DatabaseHealthCheck> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Check if database can be connected
                var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);

                if (!canConnect)
                {
                    return HealthCheckResult.Unhealthy("Database connection failed");
                }

                // Measure response time
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                _ = await _dbContext.Users.CountAsync(cancellationToken);
                stopwatch.Stop();

                var responseTime = stopwatch.ElapsedMilliseconds;

                var data = new Dictionary<string, object>
                {
                    { "ConnectionState", "Connected" },
                    { "ResponseTime", $"{responseTime}ms" },
                    { "DatabaseProvider", "SQL Server" }
                };

                if (responseTime > 1000)
                {
                    return HealthCheckResult.Degraded(
                        "Database is slow to respond",
                        data: data);
                }

                return HealthCheckResult.Healthy(
                    "Database is healthy and responsive",
                    data: data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database health check failed");
                return HealthCheckResult.Unhealthy(
                    "Database health check failed",
                    exception: ex);
            }
        }
    }
}
