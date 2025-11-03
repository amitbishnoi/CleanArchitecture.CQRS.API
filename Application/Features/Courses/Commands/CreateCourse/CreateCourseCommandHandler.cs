using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Courses.Commands.CreateCourse
{
    public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, int>
    {
        private readonly IApplicationDbContext _context;

        public CreateCourseCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            var entity = new Course
            {
                Title = request.Title,
                Description = request.Description,
                DurationInHours = request.DurationInHours
            };

            _context.Courses.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}
