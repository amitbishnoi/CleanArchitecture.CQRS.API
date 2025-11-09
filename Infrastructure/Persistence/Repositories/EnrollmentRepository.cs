using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class EnrollmentRepository : BaseRepository<Enrollment>, IEnrollmentRepository
    {
        public EnrollmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> ExistsAsync(int studentId, int courseId)
        {
            return await _context.Enrollments.AnyAsync(x => x.UserId == studentId && x.CourseId == courseId);
        }
        public async Task<IReadOnlyList<Enrollment>> GetAllWithDetailsAsync()
        {
            return await _context.Enrollments
                .Include(e => e.User)
                .Include(e => e.Course)
                .ToListAsync();
        }
        public async Task<Enrollment?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Enrollments
                .Include(e => e.User)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
