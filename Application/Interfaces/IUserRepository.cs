using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserRepository : IAsyncRepository<User>
    {
        Task<User?> GetUserWithCoursesAsync(int userId);
        Task<bool> IsEmailTakenAsync(string email);
    }
}
