using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence
{
    // This factory is required for the Entity Framework Core tools (like 'dotnet ef migrations add')
    // to create an instance of the DbContext during design time, as they cannot access
    // the application's standard dependency injection configuration.
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // 1. Setup Configuration Builder (to read appsettings.json from the API/Web project)
            // This is a common pattern to load the connection string.
            // If the connection string is only defined in the hosting project (e.g., YourApiProject),
            // you will need to manually specify the startup project when running migrations:
            // dotnet ef migrations add InitialCreate --project Infrastructure --startup-project YourApiProject
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            // 2. Configure DbContextOptions Builder
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // IMPORTANT: We must use UseSqlServer here to match the configuration in Program.cs
            // to ensure the generated migration code is correct for SQL Server.

            // --- Using SQL Server as configured in Program.cs ---
            // We attempt to get the real connection string, or use a common SQL Server placeholder.
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                                   ?? "Server=(localdb)\\mssqllocaldb;Database=DesignTimePlaceholder;Trusted_Connection=True;MultipleActiveResultSets=true";

            optionsBuilder.UseSqlServer(connectionString);
            // ---------------------------------------------------

            // 3. Return a new instance of the DbContext
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
