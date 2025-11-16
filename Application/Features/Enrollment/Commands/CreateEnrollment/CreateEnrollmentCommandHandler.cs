using Application.Interfaces;
using MediatR;

namespace Application.Features.Enrollment.Commands.CreateEnrollment
{
    public class CreateEnrollmentCommandHandler(IUnitOfWork _unitOfWork) : IRequestHandler<CreateEnrollmentCommand, int>
    {
        public async Task<int> Handle(CreateEnrollmentCommand request, CancellationToken cancellationToken)
        {
            // Validate user exists
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId).ConfigureAwait(false);
            if (user is null)
            {
                throw new Application.Common.Exceptions.ApplicationException($"User with ID {request.UserId} not found.", Application.Common.Enums.ErrorCode.UserNotFound);
            }

            // Validate course exists
            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId).ConfigureAwait(false);
            if (course is null)
            {
                throw new Application.Common.Exceptions.ApplicationException($"Course with ID {request.CourseId} not found.", Application.Common.Enums.ErrorCode.CourseNotFound);
            }

            if (await _unitOfWork.Enrollment.ExistsAsync(request.UserId, request.CourseId).ConfigureAwait(false))
            {
                return 0;
            }

            var enrollment = new Domain.Entities.Enrollment
            {
                UserId = request.UserId,
                CourseId = request.CourseId,
            };
            await _unitOfWork.Enrollment.AddAsync(enrollment).ConfigureAwait(false);
            await _unitOfWork.SaveAsync().ConfigureAwait(false);

            return enrollment.Id;
        }
    }
}
