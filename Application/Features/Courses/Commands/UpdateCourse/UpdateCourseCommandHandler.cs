using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Courses.Commands.UpdateCourse
{
    public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCourseCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.Courses.GetByIdAsync(request.Id);

            if (entity == null)
                throw new KeyNotFoundException($"Course with ID {request.Id} not found.");

            entity.Title = request.Title;
            entity.Description = request.Description;

            await _unitOfWork.Courses.UpdateAsync(entity);
            await _unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
