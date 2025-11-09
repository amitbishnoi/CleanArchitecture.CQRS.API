using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Courses.Commands.DeleteCourse
{
    public class DeleteCourseCommandHandler(IUnitOfWork _unitOfWork) : IRequestHandler<DeleteCourseCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.Courses.GetByIdAsync(request.Id);    

            if (entity == null)
                throw new KeyNotFoundException($"Course with ID {request.Id} not found.");

            await _unitOfWork.Courses.DeleteAsync(entity);
            await _unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
