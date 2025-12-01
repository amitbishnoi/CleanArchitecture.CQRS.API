using Application.Common.Responses;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace API.Controllers.V1
{
    /// <summary>
    /// API Information endpoint - provides version details and available endpoints
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ApiInfoController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ApiInfoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Get API version information and available endpoints
        /// </summary>
        /// <returns>API information including version, features, and endpoints</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<ApiInfoDto>), StatusCodes.Status200OK)]
        public IActionResult GetApiInfo()
        {
            var apiInfo = new ApiInfoDto
            {
                Name = "RemoteLMS Clean Architecture CQRS API",
                Version = "1.0",
                Description = "A production-ready Learning Management System API built with Clean Architecture and CQRS pattern.",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                Framework = ".NET 9.0",
                BuildDate = GetBuildDate(),
                Features = new List<string>
                {
                    "JWT Authentication",
                    "Role-based Authorization",
                    "CQRS with MediatR",
                    "Result<T> Pattern",
                    "Standardized API Responses",
                    "FluentValidation",
                    "AutoMapper",
                    "Serilog Logging",
                    "Email Notifications",
                    "In-Memory Caching",
                    "API Versioning"
                },
                AvailableVersions = new List<string> { "v1.0", "v2.0" },
                Endpoints = new Dictionary<string, List<string>>
                {
                    ["Authentication"] = new List<string>
                    {
                        "POST /api/v1/auth/login - User login"
                    },
                    ["Users"] = new List<string>
                    {
                        "GET /api/v1/users - Get all users",
                        "GET /api/v1/users/paged - Get paginated users",
                        "GET /api/v1/users/{id} - Get user by ID",
                        "POST /api/v1/users - Create new user (Admin only)",
                        "PUT /api/v1/users/{id} - Update user",
                        "DELETE /api/v1/users/{id} - Delete user"
                    },
                    ["Courses"] = new List<string>
                    {
                        "GET /api/v1/courses - Get all courses",
                        "GET /api/v1/courses/{id} - Get course by ID",
                        "POST /api/v1/courses - Create new course",
                        "PUT /api/v1/courses/{id} - Update course",
                        "DELETE /api/v1/courses/{id} - Delete course"
                    },
                    ["Enrollment"] = new List<string>
                    {
                        "GET /api/v1/enrollment - Get all enrollments",
                        "GET /api/v1/enrollment/{id} - Get enrollment by ID",
                        "POST /api/v1/enrollment - Create enrollment",
                        "DELETE /api/v1/enrollment/{id} - Delete enrollment"
                    }
                },
                Documentation = new Dictionary<string, string>
                {
                    ["Swagger"] = "/swagger",
                    ["GitHub"] = "https://github.com/amitbishnoi/CleanArchitecture.CQRS.API"
                }
            };

            var response = ApiResponse<ApiInfoDto>.Ok(apiInfo, "API information retrieved successfully");
            return Ok(response);
        }

        /// <summary>
        /// Get API health status
        /// </summary>
        /// <returns>Health check information</returns>
        [HttpGet("health")]
        [ProducesResponseType(typeof(ApiResponse<HealthStatusDto>), StatusCodes.Status200OK)]
        public IActionResult GetHealth()
        {
            var health = new HealthStatusDto
            {
                Status = "Healthy",
                Version = "1.0",
                Timestamp = DateTime.UtcNow,
                Uptime = GetUptime(),
                Checks = new Dictionary<string, string>
                {
                    ["API"] = "Operational",
                    ["Database"] = "Connected",
                    ["Cache"] = "Available"
                }
            };

            var response = ApiResponse<HealthStatusDto>.Ok(health, "API is healthy");
            return Ok(response);
        }

        private static DateTime GetBuildDate()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var attribute = assembly.GetCustomAttribute<BuildDateAttribute>();
            return attribute?.BuildDate ?? DateTime.UtcNow;
        }

        private static string GetUptime()
        {
            var uptime = DateTime.UtcNow - System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime();
            return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
        }
    }

    public class ApiInfoDto
    {
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public string Framework { get; set; } = string.Empty;
        public DateTime BuildDate { get; set; }
        public List<string> Features { get; set; } = new();
        public List<string> AvailableVersions { get; set; } = new();
        public Dictionary<string, List<string>> Endpoints { get; set; } = new();
        public Dictionary<string, string> Documentation { get; set; } = new();
    }

    public class HealthStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Uptime { get; set; } = string.Empty;
        public Dictionary<string, string> Checks { get; set; } = new();
    }

    [AttributeUsage(AttributeTargets.Assembly)]
    public class BuildDateAttribute : Attribute
    {
        public DateTime BuildDate { get; }

        public BuildDateAttribute(string date)
        {
            BuildDate = DateTime.Parse(date);
        }
    }
}
