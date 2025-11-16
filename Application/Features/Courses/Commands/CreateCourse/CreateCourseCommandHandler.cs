using Application.Common.Enums;
using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Courses.Commands.CreateCourse
{
    public class CreateCourseCommandHandler(IUnitOfWork _unitOfWork) : IRequestHandler<CreateCourseCommand, int>
    {
        public async Task<int> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            // Verify instructor exists
            var instructor = await _unitOfWork.Users.GetByIdAsync(request.InstructorId);
            if (instructor == null)
                throw new Common.Exceptions.ApplicationException(
                    $"Instructor with ID {request.InstructorId} not found.",
                    ErrorCode.InstructorNotFound);

            var entity = new Course
            {
                Title = request.Title,
                Description = request.Description,
                InstructorId = request.InstructorId
            };

            await _unitOfWork.Courses.AddAsync(entity);
            await _unitOfWork.SaveAsync();
                
            return entity.Id;
        }
    }
}
