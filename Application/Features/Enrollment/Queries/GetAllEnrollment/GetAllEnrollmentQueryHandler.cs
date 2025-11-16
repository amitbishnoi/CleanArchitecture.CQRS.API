using Application.Features.Enrollment.Dtos;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Enrollment.Queries.GetAllEnrollment
{
    public class GetAllEnrollmentQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetAllEnrollmentQuery, List<EnrollmentDto>>
    {
        public async Task<List<EnrollmentDto>> Handle(GetAllEnrollmentQuery request, CancellationToken cancellationToken)
        {
            var enrollments = await _unitOfWork.Enrollment.GetPagedEnrollmentsAsync(
                request.Pagination.PageNumber,
                request.Pagination.PageSize
            );
            var enrollmentDtos = enrollments.Select(enrollment => new EnrollmentDto
            {
                Id = enrollment.Id,
                UserName = enrollment.User.Name,
                CourseTitle = enrollment.Course.Title,
            }).ToList();
            return enrollmentDtos;
        }
    }
}
