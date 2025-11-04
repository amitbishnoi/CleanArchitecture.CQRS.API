using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICourseRepository : IAsyncRepository<Course>
    {
        Task<IReadOnlyList<Course>> GetCoursesWithUsersAsync();
        Task<Course?> GetByTitleAsync(string title);
    }
}
