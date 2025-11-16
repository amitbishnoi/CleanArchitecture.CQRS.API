using Application.Features.Enrollment.Dtos;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Enrollment.Queries.GetEnrollmentById
{
    public class GetEnrollmentByIdQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetEnrollmentByIdQuery, EnrollmentDto?>
    {
        public async Task<EnrollmentDto?> Handle(GetEnrollmentByIdQuery request, CancellationToken cancellationToken)
        {
            var enrollment = await _unitOfWork.Enrollment.GetByIdWithDetailsAsync(request.Id);
            if (enrollment == null)
            {
                return null;
            }
            var enrollmentDto = new EnrollmentDto
            {
                Id = enrollment.Id,
                UserName = enrollment.User.Name,
                CourseTitle = enrollment.Course.Title,
            };
            return enrollmentDto;
        }
    }
}
