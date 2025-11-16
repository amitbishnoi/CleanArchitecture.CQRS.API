using Domain.Entities;

namespace Application.Interfaces
{
    public interface IEnrollmentRepository : IAsyncRepository<Enrollment>
    {
        Task<bool> ExistsAsync(int studentId, int courseId);
        Task<IReadOnlyList<Enrollment>> GetAllWithDetailsAsync();
        Task<Enrollment?> GetByIdWithDetailsAsync(int Id);
        Task<IReadOnlyList<Enrollment>> GetPagedEnrollmentsAsync(int pageNumber, int pageSize);
    }
}
