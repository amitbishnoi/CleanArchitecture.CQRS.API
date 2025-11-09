using Application.Features.Enrollment.Dtos;
using MediatR;

namespace Application.Features.Enrollment.Queries.GetAllEnrollment
{
    public class GetAllEnrollmentQuery : IRequest<List<EnrollmentDto>> { }
}
