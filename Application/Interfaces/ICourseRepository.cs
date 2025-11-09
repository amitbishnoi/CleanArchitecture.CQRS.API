using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICourseRepository : IAsyncRepository<Course>
    {
        Task<IReadOnlyList<Course>> GetCoursesWithUsersAsync();
        Task<Course?> GetByTitleAsync(string title);
        Task<IReadOnlyList<Course>> GetPagedCoursesAsync(int pageNumber, int pageSize, string? searchTerm);
    }
}
