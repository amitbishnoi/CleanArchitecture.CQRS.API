using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class CourseRepository : BaseRepository<Course>, ICourseRepository
    {
        public CourseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Course>> GetCoursesWithUsersAsync()
        {
            return await _context.Courses
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Course?> GetByTitleAsync(string title)
        {
            return await _context.Courses
                .FirstOrDefaultAsync(c => c.Title.ToLower() == title.ToLower());
        }
    }
}
