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

        public async Task<IReadOnlyList<Course>> GetPagedCoursesAsync(int pageNumber, int pageSize, string? searchTerm)
        {
            var query = _context.Courses.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x => x.Title.Contains(searchTerm) || x.Description.Contains(searchTerm));
            }

            return await query
                .OrderBy(x => x.Title)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
