using Application.Interfaces;
using Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IUserRepository Users { get; }
        public ICourseRepository Courses { get; }

        public UnitOfWork(
            ApplicationDbContext context,
            IUserRepository userRepository,
            ICourseRepository courseRepository)
        {
            _context = context;
            Users = userRepository;
            Courses = courseRepository;
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
