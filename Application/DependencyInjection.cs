using Application.Behaviors;
using Application.Mappings;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register MediatR with assembly scanning
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            
            // Register ResponseWrappingBehavior to wrap all handler responses in ApiResponse<T>
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ResponseWrappingBehavior<,>));
            
            // Register AutoMapper with MappingProfile
            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
            
            // Register FluentValidation validators
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            
            // Register in-memory caching
            services.AddMemoryCache();

            return services;
        }
    }
}
