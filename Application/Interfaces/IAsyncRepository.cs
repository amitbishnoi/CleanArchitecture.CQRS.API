using Domain.Entities;
using System.Linq.Expressions;

namespace Application.Interfaces
{
    public interface IAsyncRepository<T> where T : BaseEntity
    {
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
        Task<T?> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<T>> GetWhereAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
