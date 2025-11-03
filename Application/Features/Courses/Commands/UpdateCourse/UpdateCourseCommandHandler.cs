using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Courses.Commands.UpdateCourse
{
    public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public UpdateCourseCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Courses.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"Course with ID {request.Id} not found.");

            entity.Title = request.Title;
            entity.Description = request.Description;
            entity.DurationInHours = request.DurationInHours;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
