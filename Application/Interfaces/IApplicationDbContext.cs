using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Course> Courses { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
