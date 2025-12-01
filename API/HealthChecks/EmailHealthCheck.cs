using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Mail;

namespace API.HealthChecks
{
    /// <summary>
    /// Custom health check for email service connectivity
    /// </summary>
    public class EmailHealthCheck : IHealthCheck
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailHealthCheck> _logger;

        public EmailHealthCheck(IConfiguration configuration, ILogger<EmailHealthCheck> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var smtpServer = emailSettings["SmtpServer"];
                var port = emailSettings["Port"];

                if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(port))
                {
                    return Task.FromResult(HealthCheckResult.Degraded(
                        "Email settings not configured properly"));
                }

                var data = new Dictionary<string, object>
                {
                    { "SmtpServer", smtpServer },
                    { "Port", port },
                    { "Status", "Configured" }
                };

                // Note: Full SMTP connectivity test would require actual connection
                // which might be slow and trigger security alerts, so we just validate config
                return Task.FromResult(HealthCheckResult.Healthy(
                    "Email service is configured",
                    data: data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email health check failed");
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    "Email health check failed",
                    exception: ex));
            }
        }
    }
}
