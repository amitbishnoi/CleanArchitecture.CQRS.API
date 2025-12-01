using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Configuration
{
    /// <summary>
    /// Configures Swagger generation options for API versioning.
    /// Automatically creates a Swagger document for each API version.
    /// </summary>
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            // Add a swagger document for each discovered API version
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            }
        }

        private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var text = new System.Text.StringBuilder();
            text.Append("A production-ready Learning Management System API built with Clean Architecture and CQRS pattern.");
            text.Append("<br/><br/>");
            text.Append("<strong>Features:</strong>");
            text.Append("<ul>");
            text.Append("<li>✅ Clean Architecture with 4-layer separation</li>");
            text.Append("<li>✅ CQRS with MediatR for command/query segregation</li>");
            text.Append("<li>✅ JWT Bearer Authentication & Role-based Authorization</li>");
            text.Append("<li>✅ Result&lt;T&gt; pattern for functional error handling</li>");
            text.Append("<li>✅ Comprehensive health checks for system monitoring</li>");
            text.Append("<li>✅ FluentValidation for input validation</li>");
            text.Append("<li>✅ AutoMapper for object mapping</li>");
            text.Append("<li>✅ Serilog for structured logging</li>");
            text.Append("<li>✅ API Versioning support (URL, Header, Query String)</li>");
            text.Append("</ul>");
            text.Append("<br/>");
            text.Append("<strong>Health Checks:</strong> <a href='/health'>/health</a><br/>");
            text.Append("<strong>Health UI:</strong> <a href='/health-ui'>/health-ui</a>");

            var info = new OpenApiInfo()
            {
                Title = "RemoteLMS Clean Architecture CQRS API",
                Version = description.ApiVersion.ToString(),
                Description = text.ToString(),
                Contact = new OpenApiContact
                {
                    Name = "Amit Bishnoi",
                    Email = "amitbishnoi246@gmail.com",
                    Url = new Uri("https://github.com/amitbishnoi")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                },
                TermsOfService = new Uri("https://example.com/terms")
            };

            if (description.IsDeprecated)
            {
                info.Description += "<br/><br/><strong style='color: red;'>⚠️ This API version has been deprecated. Please migrate to the latest version.</strong>";
            }

            return info;
        }
    }
}
