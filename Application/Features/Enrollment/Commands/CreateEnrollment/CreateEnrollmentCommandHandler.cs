using Application.Interfaces;
using MediatR;

namespace Application.Features.Enrollment.Commands.CreateEnrollment
{
    public class CreateEnrollmentCommandHandler(IUnitOfWork _unitOfWork) : IRequestHandler<CreateEnrollmentCommand, int>
    {
        public async Task<int> Handle(CreateEnrollmentCommand request, CancellationToken cancellationToken)
        {
            if( await _unitOfWork.Enrollment.ExistsAsync(request.UserId, request.CourseId).ConfigureAwait(false))
            {
                return 0;
            }

            var enrollment = new Domain.Entities.Enrollment
            {
                UserId = request.UserId,
                CourseId = request.CourseId,
            };
            await _unitOfWork.Enrollment.AddAsync(enrollment);
            await _unitOfWork.SaveAsync();

            return enrollment.Id;
        }
    }
}
